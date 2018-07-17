using ColonyConcierge.APIData.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Shared
{
    class JSONDeserializer : IDeserializer
    {

        class MyConverter<T> : JsonConverter
        {
            protected object Create(Type objectType, JObject jsonObject)
            {
                JProperty prop = jsonObject.Property("__type");

                if (prop != null)
                {
                    string className = prop.First().ToString();
                    var parts = className.Split('#');
                    var qualName = string.Format("{0}.{1}", parts[1], parts[0]).Replace(":", "");
                    Type type = objectType.Assembly.GetType(qualName);
                    var obj = Activator.CreateInstance(type);
                    return obj;
                    //return Activator.CreateInstance(objectType);

                }
                else
                {

                    return Activator.CreateInstance(objectType);
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var json = JObject.Load(reader);
                var obj = Create(objectType, json);
                serializer.Populate(json.CreateReader(), obj);
                return obj;
            }

            public override bool CanConvert(Type objectType)
            {
                //bool convertable = typeof(InheritedTypeBase).IsAssignableFrom(objectType);
                //return convertable;
                return typeof(InheritedTypeBase).IsAssignableFrom(objectType);
            }         

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        };

        //class inspired from https://gist.github.com/petermorlion/c92e3af4ecf256d4b66c
        class MyDictionaryConverter<T> : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType.IsGenericType && (objectType.GetGenericTypeDefinition() == typeof(IDictionary<,>) || objectType.GetGenericTypeDefinition() == typeof(Dictionary<,>));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (!CanConvert(objectType))
                    throw new Exception(string.Format("This converter is not for {0}.", objectType));

                var keyType = objectType.GetGenericArguments()[0];
                var valueType = objectType.GetGenericArguments()[1];
                var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                var result = (IDictionary)Activator.CreateInstance(dictionaryType);

                if (reader.TokenType == JsonToken.Null)
                    return null;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        return result;
                    }

                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        AddObjectToDictionary(reader, result, serializer, keyType, valueType);
                    }
                }

                return result;
            }

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            private void AddObjectToDictionary(JsonReader reader, IDictionary result, Newtonsoft.Json.JsonSerializer serializer, Type keyType, Type valueType)
            {
                object key = null;
                object value = null;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndObject && key != null)
                    {
                        result.Add(key, value);
                        return;
                    }

                    var propertyName = reader.Value.ToString();
                    if (propertyName == "Key")
                    {
                        reader.Read();
                        key = serializer.Deserialize(reader, keyType);
                    }
                    else if (propertyName == "Value")
                    {
                        reader.Read();
                        value = serializer.Deserialize(reader, valueType);
                    }
                }
            }

        }

        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        
        public string DateFormat
        {
            get;
            set;
        }

        public string Namespace
        {
            get;
            set;
        }

        public string RootElement
        {
            get;
            set;
        }

        public JSONDeserializer()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer();

        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            using (var stringReader = new StringReader(response.Content))
            {
                JsonTextReader jsonReader = new JsonTextReader(stringReader);
                JsonSerializerSettings settings = new JsonSerializerSettings();

                T result = JsonConvert.DeserializeObject<T>(response.Content, new MyConverter<T>(), new MyDictionaryConverter<T>() );

                return result;
            }
            
        
        }


    }
}

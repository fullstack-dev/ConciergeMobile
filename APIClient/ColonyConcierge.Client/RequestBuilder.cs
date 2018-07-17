using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client.PlatformServices;
using ColonyConcierge.Client.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// Helps to construct a rest request.
    /// This class is never meant to be referenced or instanced directly, but an instance can be created 
    /// from a Connector object
    /// </summary>
    /// <remarks>
    /// This class can be used to construct REST calls to the API server, however, it is not intended to be instanced directly.
    /// The <see cref="ColonyConcierge.Client.DataSources">ObjectData API</see> makes extensive use of this class internally to create API calls.
    /// </remarks>
    public class RequestBuilder<T> where T : new() 
    {

        protected RestRequest _request;

        protected Connector _connector;
        string _url;

        internal RequestBuilder(Connector connector, string url, RestMethod method)
        {
            _connector = connector;
            _url = url;

            _request = Provider.G.Rest.CreateRequest(url, method);

            //_request = new RestSharp.RestRequest(url) { 
            //    RequestFormat = RestSharp.DataFormat.Json,
            //    Method = method,
            //    JsonSerializer = new JSONSerializer()
                
            //};

            //TODO was this actually needed?
            //_request.RequestFormat = RestSharp.DataFormat.Json;

        }

        private RequestBuilder(RequestBuilder<T> other)
        {
            _connector = other._connector;
            _url = other._url;
            _request = other._request;
        }

        /// <summary>
        /// Adds a named parameter to a request, in the form or a url segment
        /// The value of the parameter can be any .Net type, but "ToString" will be called on it
        /// </summary>
        /// <param name="name">The name of the parameter, this has to match the '{name} portion of the segment</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        public RequestBuilder<T> Parameter(string name, object value)
        {
            _request.AddUrlSegment(name, SmartToString( value));
            return this;
        }

        /// <summary>
        /// Adds a main parameter to a request, in the form of the http request body.
        /// There can be only one main parameter, and it is typically a structure.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public RequestBuilder<T> Parameter(object value)
        {
            _request.AddBody(value);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public RequestBuilder<T> Query(string name, object value)
        {
            if (value != null)
            {
                _request.AddQueryParameter(name, SmartToString(value));
            }
            return this;
        }



        public T Run()
        {

            return _connector.ExecuteRequest<T>(_request);

        }

        public async Task<T> RunAsync()
        {
            return await _connector.ExecutRequestAsync<T>(_request);
        }


        /// <summary>
        /// Implicit cast to a Data object. This will indirectly throw a <see cref="ServerSideErrorException"/> if the 
        /// server encounters an error that prevents it from creating the object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static implicit operator T (RequestBuilder<T> source)        
        {
            return source.Run();
        }

        public static implicit operator Task<T> (RequestBuilder<T> source)
        {
            return source.RunAsync();
        }
        

        /// <summary>
        /// Implicit cast to a <see cref="DataResult"/> object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static implicit operator DataResult<T>(RequestBuilder<T> source)
        {
            return source.RunResult();
        }


        public static implicit operator Task<DataResult<T>>(RequestBuilder<T> source)
        {
            return source.RunResultAsync();
        }

        internal DataResult<T> RunResult()
        {
            return _connector.ExecuteResultRequest<T>(_request);
        }

        internal async Task<DataResult<T>> RunResultAsync()
        {
            return await _connector.ExecuteResultRequestAsync<T>(_request);
        }

        internal T Raw<T>() where T : Result, new()
        {
            return _connector.ExecuteRawRequest<T>(_request);
        }

        string SmartToString(object value)
        {
            if (value != null)
            {
                if (value is string)
                {
                    return value as string;
                }
                if (value is DateTime)
                {
                    throw new ArgumentException("Do not pass a DateTime object to the API, use either a Date object, a TimeOfDay object, or a TimeStamp object!");
                }
                if (value is IUrlFormatable)
                {
                    return (value as IUrlFormatable).GetUrlSafeString();
                }
                else if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()) )
                {
                    return CollectionToString(value as IEnumerable);
                }
                else
                {
                    return value.ToString();
                }
            }
            return null;
        }

        string CollectionToString(IEnumerable obj)
        {
            var strings = string.Join(",", (from object o in obj
                                            select SmartToString(o)));

            return strings;
        }


    }
}

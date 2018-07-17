using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    class ContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            //slight hack to ensure that the special metadata member "__type" is always first
            if (property.PropertyName == "__type")
            {
                property.Order = -1000; //that should make it ALWAYS first
            }
            //return base.CreateProperty(member, memberSerialization);
            return property;
        }

        protected override JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, System.Reflection.ParameterInfo parameterInfo)
        {
            var property = base.CreatePropertyFromConstructorParameter(matchingMemberProperty, parameterInfo);
            //slight hack to ensure that the special metadata member "__type" is always first
            if (property.PropertyName == "__type")
            {
                property.Order = -1000; //that should make it ALWAYS first
            }
            return property;
        }
    }
}

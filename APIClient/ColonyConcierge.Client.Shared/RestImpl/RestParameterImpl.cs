using ColonyConcierge.Client.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColonyConcierge.Client.Shared.RestImpl
{
    class RestParameterImpl : RestParameter
    {

        #region Private Data
        RestSharp.Parameter _realParameter;
        #endregion

        public override string Name
        {
            get
            {
                return _realParameter.Name;
            }
        }

        public override RestParameterType Type
        {
            get
            {

                switch(_realParameter.Type)
                {
                    case RestSharp.ParameterType.Cookie:
                    {
                        return RestParameterType.QueryString;
                    }
                    case RestSharp.ParameterType.RequestBody:
                    {
                        return RestParameterType.RequestBody;
                    }
                    case RestSharp.ParameterType.UrlSegment:
                    {
                        return RestParameterType.UrlSegment;
                    }
                    default:
                    {
                        return RestParameterType.Unknown;
                    }
                }


            }
        }

        public override object Value
        {
            get
            {
                return _realParameter.Value;
            }
        }


        public RestParameterImpl(RestSharp.Parameter realParameter)
        {
            _realParameter = realParameter;
        }
    }
}

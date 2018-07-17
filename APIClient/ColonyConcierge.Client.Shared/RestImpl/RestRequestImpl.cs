using ColonyConcierge.Client.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonyConcierge.Client.Shared.RestImpl
{
    class RestRequestImpl : RestRequest
    {


        #region Private Data
        RestSharp.RestRequest _realRequest;
        RestMethod _method;
        #endregion



        public override RestMethod Method
        {
            get
            {
                return _method;
            }
        }

        public override List<RestParameter> Parameters
        {
            get
            {
                var parameters = (from p in _realRequest.Parameters
                                  select new RestParameterImpl(p)).Cast<RestParameter>().ToList();

                return parameters;
            }
        }

        public override string Resource
        {
            get
            {
                return _realRequest.Resource;
            }
        }

        internal RestSharp.RestRequest RealRequest
        {
            get
            {
                return _realRequest;
            }
        }


        public RestRequestImpl(string path, RestMethod method)
        {

            RestSharp.Method nativeMethod;
            switch (method)
            {
                case RestMethod.GET:
                {
                    nativeMethod = RestSharp.Method.GET;
                    break;
                }
                case RestMethod.PUT:
                {
                    nativeMethod = RestSharp.Method.PUT;
                    break;
                }
                case RestMethod.POST:
                {
                    nativeMethod = RestSharp.Method.POST;
                    break;
                }
                case RestMethod.DELETE:
                {
                    nativeMethod = RestSharp.Method.DELETE;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException($"Rest method: '{method}' not handled");
                }

            }

            _realRequest = new RestSharp.RestRequest(path, nativeMethod)
            {
                RequestFormat = RestSharp.DataFormat.Json,
                JsonSerializer = new JSONSerializer()
            };

            _method = method;
            
        }

        public override RestRequest AddBody(object obj)
        {
            _realRequest.AddBody(obj);
            return this;
        }

        public override RestRequest AddHeader(string name, string value)
        {
            _realRequest.AddHeader(name, value);
            return this;
        }

        public override RestRequest AddQueryParameter(string name, string value)
        {
            _realRequest.AddQueryParameter(name, value);
            return this;
        }

        public override RestRequest AddUrlSegment(string name, string value)
        {
            _realRequest.AddUrlSegment(name, value);
            return this;
        }
    }
}

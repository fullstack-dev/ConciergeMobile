using ColonyConcierge.Client.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace ColonyConcierge.Client.Shared.RestImpl
{
    class RestResponseImpl<T> : RestResponse<T>
    {


        #region Private Data
        RestSharp.IRestResponse<T> _realResponse;
        #endregion

        public override string Content
        {
            get
            {
                return _realResponse.Content;
            }
        }

        public override T Data
        {
            get
            {
                return _realResponse.Data;
            }
        }

        public override HttpStatusCode StatusCode
        {
            get
            {
                return _realResponse.StatusCode;
            }
        }

        public RestResponseImpl(RestSharp.IRestResponse<T> realResponse)
        {
            _realResponse = realResponse;
        }
    }
}

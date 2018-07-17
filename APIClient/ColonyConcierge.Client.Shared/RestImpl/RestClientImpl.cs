using ColonyConcierge.Client.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Shared.RestImpl
{
    class RestClientImpl : RestClient
    {

        #region Private Data
        RestSharp.RestClient _realClient;
        #endregion

        public RestClientImpl(string url)
        {
            _realClient = new RestSharp.RestClient(url);

            _realClient.AddHandler("application/json", new JSONDeserializer());
        }

        public override RestResponse<T> Execute<T>(RestRequest request) 
        {
            RestRequestImpl reqImple = request as RestRequestImpl;
            var result = _realClient.Execute<T>(reqImple.RealRequest);
            return new RestResponseImpl<T>(result);
        }

        public async override Task<RestResponse<T>> ExecuteTaskAsync<T>(RestRequest request)
        {

            RestRequestImpl reqImple = request as RestRequestImpl;

            var result = await _realClient.ExecuteTaskAsync<T>(reqImple.RealRequest);

            return new RestResponseImpl<T>(result);
        }
    }
}

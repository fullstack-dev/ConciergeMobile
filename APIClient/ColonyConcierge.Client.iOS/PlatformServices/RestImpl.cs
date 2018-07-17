using ColonyConcierge.Client.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyConcierge.Client.Rest;
using ColonyConcierge.Client.Shared.RestImpl;

namespace ColonyConcierge.Client.iOS.PlatformServices
{
    class RestImpl : IRest
    {
        public RestClient CreateClient(string url)
        {
            var retVal = new RestClientImpl(url);

            return retVal;
        }

        public RestRequest CreateRequest(string url, RestMethod method)
        {
            var retVal = new RestRequestImpl(url, method);

            return retVal;
        }
    }
}

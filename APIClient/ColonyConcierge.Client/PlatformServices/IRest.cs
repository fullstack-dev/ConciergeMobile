using ColonyConcierge.Client.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.PlatformServices
{
    public interface IRest
    {

        RestRequest CreateRequest(string url, RestMethod method);

        RestClient CreateClient(string url);
    }
}

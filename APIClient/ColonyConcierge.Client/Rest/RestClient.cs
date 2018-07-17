using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Rest
{
    public abstract class RestClient
    {

        public abstract RestResponse<T> Execute<T>(RestRequest request) where T : new();


        public abstract Task<RestResponse<T>> ExecuteTaskAsync<T>(RestRequest request) where T : new();
    }
}

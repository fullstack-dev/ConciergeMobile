using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Rest
{
    public abstract class RestResponse<T>
    {

        public abstract string Content
        {
            get;
        }

        public abstract T Data
        {
            get;
        }

        public abstract System.Net.HttpStatusCode StatusCode
        {
            get;
        }

    }
}

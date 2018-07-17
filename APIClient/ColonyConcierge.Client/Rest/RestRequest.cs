using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Rest
{
    public abstract class RestRequest
    {

        public abstract string Resource
        {
            get;
        }

        public abstract RestMethod Method
        {
            get;
        }

        public abstract List<RestParameter> Parameters
        {
            get;
        }


        public abstract RestRequest AddUrlSegment(string name, string value);

        public abstract RestRequest AddQueryParameter(string name, string value);

        public abstract RestRequest AddBody(object obj);

        public abstract RestRequest AddHeader(string name, string value);


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Rest
{
    public abstract class RestParameter
    {

        public abstract string Name
        {
            get;
        }

        public abstract object Value
        {
            get;
        }

        public abstract RestParameterType Type
        {
            get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    public class WrongHttpVerbException : Exception
    {

        #region Properties
        public string VerbUsed
        {
            get;
            private set;
        }
        #endregion

        public WrongHttpVerbException(string verbUsed, string url) : 
            base(string.Format("The server does not accept the provided verb '{0}' for '{1}'", verbUsed, url))
        {
            VerbUsed = verbUsed;
        }
    }
}

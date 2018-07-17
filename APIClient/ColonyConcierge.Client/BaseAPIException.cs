using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// The base exceptions type for all custom exceptions thrown by the CC API client dll.
    /// Some exceptions represent problems on the server side, and some exceptions may originate within
    /// logic on the client itself.
    /// </summary>
    public class BaseAPIException : Exception
    {
        ///<remarks/>
        protected BaseAPIException()
        {

        }

        ///<remarks/>
        protected BaseAPIException(string message)
            : base(message)
        {

        }
    }
}

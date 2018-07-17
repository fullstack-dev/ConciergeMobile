using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// An exception thrown by the client API wrapper when it detects that an API call on the server
    /// had an authorization failure
    /// </summary>
    public class AuthorizationException : BaseAPIException
    {
        /// <remarks/>
        public AuthorizationException(string message)
            : base("Server Authentication Error: " + message)
        {
            
        }
    }
}

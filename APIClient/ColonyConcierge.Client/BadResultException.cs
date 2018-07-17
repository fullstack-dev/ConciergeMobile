using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// Exception Thrown by the client API when it detects a bad result, or a failed data validation on the server
    /// </summary>
    public class BadResultException : BaseAPIException
    {
        /// <remarks/>
        public BadResultException(string message)
            : base(message)
        {

        }
    }
}

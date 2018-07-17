using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// Exception Thrown by the client API when the server reports that a called method exists, but is not implemented.
    /// </summary>
    public class UnImplementedMethodException : ServerSideErrorException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        public UnImplementedMethodException(ErrorObject error, string message)
            : base(error, message)
        {
           
        }
    }
}

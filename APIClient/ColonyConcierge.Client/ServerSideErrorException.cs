using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// Exception Thrown by the client API when it detects an error on the server side.
    /// </summary>
    public class ServerSideErrorException : BaseAPIException
    {
        #region Properties
        /// <summary>
        /// The diagnostic error object produced by the server
        /// </summary>
        public ErrorObject ServerError { get; private set; }
        #endregion

        ///<remarks/>
        public ServerSideErrorException(ErrorObject error, string message) : base("Server Side Error: " + message)
        {
            ServerError = error;
        }


    }
}

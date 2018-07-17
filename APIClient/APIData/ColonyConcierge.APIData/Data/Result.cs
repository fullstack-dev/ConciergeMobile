using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// The base class for all data returned from the API server.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Created a new result object.
        /// </summary>
        public Result()
        {

        }

        /// <summary>
        /// Creates a new Result object, allowing the valid flag to be set on the object.
        /// </summary>
        /// <param name="ok"></param>
        public Result(bool ok)
        {
            OK = ok;
        }

        /// <summary>
        /// Indicates whether the call returnd valid data or not.
        /// </summary>
        public bool OK { get; set; }

        /// <summary>
        /// An optional message form the server that can provide additional information from the server. This is often <code>null</code>
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// An <see cref="ErrorObject"/> object that provide detail information about any error that occured.
        /// </summary>
        public ErrorObject Error { get; set; }

        public bool AuthorizationFailed { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This class provides additional information about server-side errors that may have occurred while processing an API request.
    /// </summary>
    public class ErrorObject
    {
        /// <summary>
        /// The stack trace where a fatal server--side error occurred.
        /// <note>This will be empty coming from production servers</note>
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Any server side methods that are currently placeholders will set this flag.
        /// </summary>
        public bool NotImplemented { get; set; }
    }
}

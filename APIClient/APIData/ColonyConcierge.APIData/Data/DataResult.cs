using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{

    /// <summary>
    /// Base class for API's that return "Data", (which is most APIs)
    /// </summary>
    public class DataResult : Result
    {
        /// <summary>
        /// Helper method to return an DataResult object with a valid payload.
        /// </summary>
        /// <typeparam name="T">The type of the payload object.</typeparam>
        /// <param name="data">The payload data.</param>
        /// <returns></returns>
        public static DataResult<T> Good<T>(T data)
        {
            return new DataResult<T> { OK = true, Data = data };
        }

    }

    /// <summary>
    /// Base class for API's that return "Data", (which is most APIs)
    /// </summary>
    /// <typeparam name="T">The wrapped object type</typeparam>
    /// <remarks>
    /// Most API's return a DataResult object, with some sort of "payload" data. 
    /// The purpose of the wrapper is to allow for extra metadata
    /// and error conditions.
    /// </remarks>
    public class DataResult<T> : Result
    {
        public DataResult() : base(true)
        {

        }

        /// <summary>
        /// The payload data object returned by the server API. This may be <code>null</code>
        /// </summary>
        public T Data
        {
            get;
            set;
        }

        /// <summary>
        /// Helper method to return an DataResult indicating an error condition. This is used on the server side.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="formatObjects">optional objects to use in format string</param>
        /// <returns>a DataResult object that indicates an error occurred on the server.</returns>
        public static DataResult<T> Bad(string message, params object[] formatObjects)
        {
            return new DataResult<T>() { OK = false, Message = string.Format( message, formatObjects) };
        }

        /// <summary>
        /// Helper method to return an DataResult indicating an failed authorization condition. This is used on the server side.
        /// </summary>
        /// <param name="failureDetails"></param>
        /// <param name="callerName">Set by the compiler, do not use</param>
        /// <returns>a DataResult object that indicates an error occurred on the server.</returns>
        public static DataResult<T> AuthFail(string failureDetails, string callerName)
        {
            return new DataResult<T>() { OK = false, Message = string.Format("Authorization Failed for {0} because {1}", callerName, failureDetails),
                AuthorizationFailed = true };
        }

        /// <summary>
        /// Helper method to return an DataResult indicating that the called method is not implemented. This is used on the server side.
        /// </summary>
        /// <param name="callerName">Set by the compiler, do not use</param>
        /// <returns>a DataResult object that indicates an error occurred on the server.</returns>
        public static DataResult<T> NotImplemented([CallerMemberName] string callerName = "")
        {
            return new DataResult<T>() { OK = false, Message = string.Format("Method '{0}' is not implemented!", callerName),
            Error = new ErrorObject() {NotImplemented = true} };
        }
    }

}

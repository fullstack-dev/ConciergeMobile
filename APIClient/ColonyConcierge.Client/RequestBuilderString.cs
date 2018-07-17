using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// Helper class that builds REST requests. This is never intended to be instanced directly, but created by a Connector object.
    /// </summary>
    public class RequestBuilderString : RequestBuilder<object>
    {

        internal RequestBuilderString(Connector connector, string url, RestMethod method)
            : base (connector, url, method)
        {
        }

        /// <summary>
        /// Adds a named parameter to a request, in the form or a url segment
        /// The value of the parameter can be any .Net type, but "ToString" will be called on it
        /// </summary>
        /// <param name="name">The name of the parameter, this has to match the '{name} portion of the segment</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        public RequestBuilderString Parameter(string name, object value)
        {
            _request.AddUrlSegment(name, value.ToString());
            return this;
        }

        /// <summary>
        /// Adds a main parameter to a request, in the form of the http request body.
        /// There can be only one main parameter, and it is typically a structure.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public RequestBuilderString Parameter(object value)
        {
            _request.AddBody(value);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public RequestBuilderString Query(string name, object value)
        {
            if (value != null)
            {
                _request.AddQueryParameter(name, value.ToString());
            }
            return this;
        }

        public async Task<string> RunAsync()
        {
            var resString = await _connector.ExecutRequestAsync<object>(_request);
            return resString as string;
        }

        /// <summary>
        /// Implicit cast to a Data object. This will indirectly throw a <see cref="ServerSideErrorException"/> if the 
        /// server encounters an error that prevents it from creating the object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static implicit operator string(RequestBuilderString source)
        {
            return source.Run() as string;
        }

        /// <summary>
        /// Implicit cast to a <see cref="DataResult"/> object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static implicit operator DataResult<object>(RequestBuilderString source)
        {
            return source.RunResult();
        }
    
    }
}

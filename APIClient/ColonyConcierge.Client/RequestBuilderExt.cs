using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// Extension method helpers for request builder
    /// </summary>
    public static class RequestBuilderExt
    {
        /// <summary>
        /// Executes a "Raw" (non-wrapped in a <see cref="DataResult{T}"/>) object
        /// </summary>
        /// <typeparam name="T">The <see cref="Result"/>-derived object to return</typeparam>
        /// <param name="requestBuilder"></param>
        /// <returns>The result object from the server</returns>
        public static T Raw<T>(this RequestBuilder<T> requestBuilder) where T : Result, new()
        {
            return requestBuilder.Raw<T>();
        }

        public static async Task<T> RawAsync<T>(this Task<T> requestBuilder)  where T : Result, new()
        {
            return await requestBuilder;
        }


    }
}

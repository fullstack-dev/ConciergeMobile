using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Returns information that a client can use to determine if it is up to date.
    /// The server does not have any logic around this data, it is simple information that 
    /// is reported to clients that they can take the appropriate action on.
    /// </summary>
    public class ClientVersioningInfo
    {
        public string MinimumRecommendedVersion { get; set; }

        public string MinimumRequiredVersion { get; set; }

        /// <summary>
        /// The platform, if any on the update info record. This may be blank, or may not match exactly the parameter passed, if the back end determines
        /// the match was "close enough" or compatible with thee request.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// This is some sort of url or other string that inidcates to the client app how to perform and update.
        /// For an Android app, for example, this might be a link to the play store. 
        /// </summary>
        public string UpdateUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Describes the user level of customer users.
    /// </summary>
    public enum CustomerUserLevel
    {
        /// <summary>
        /// This value is returned for user accounts that are disabled, or not customers
        /// </summary>
        Other,
        /// <summary>
        /// This is a new user who has not been approved yet
        /// </summary>
        Pending,
        /// <summary>
        /// This is a user who can schedule services, but typically not manage payments or other account info
        /// </summary>
        Approved,
        /// <summary>
        /// This is typically an account owner or co-owner, who can manage payment methods, change plans, etc.
        /// </summary>
        Authorized
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents an account, to which one or more user can belong. There is only one "Primary" user.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// The unique ID of this account
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// The user ID of the primary user of this account.
        /// </summary>
        public int PrimaryUserID
        {
            get;
            set;
        }

        /// <summary>
        /// The ID of the current plan the user is subscribed to. This should be considered a "read only" property
        /// from the server. Use the appropriate server APIs to request a plan change.
        /// </summary>
        public int SubscriptionPlanID
        {
            get;
            set;
        }

        /// <summary>
        /// The time that the current plan will expire. On the next day, the plan will be renewed, or if renewal options have
        /// been changed, the plan will be modified.
        /// </summary>
        public SimpleDate? PlanExpiration
        {
            get;
            set;
        }

        /// <summary>
        /// This is set to 'true' is the account has an active service subscription. This is "read only", set on the server side.
        /// </summary>
        public bool IsServiceActive
        {
            get;
            set;
        }

        /// <summary>
        /// The "home" timezone of the user. 
        /// </summary>
        public string TimeZoneID
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the account ID as a string
        /// </summary>
        /// <returns>a string value representing the ID</returns>
        public override string ToString()
        {
            return ID.ToString();
        }
    }
}

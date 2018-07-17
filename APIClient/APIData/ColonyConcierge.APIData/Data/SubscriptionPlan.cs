using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class representing a subscription plan
    /// </summary>
    public class SubscriptionPlan
    {
        public int ID { get; set; }

        /// <summary>
        /// The name of the SubscriptionPlan
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The monthly fee for this subscription.
        /// If <b>null</b>, then the fee for this plan is variable.
        /// </summary>
        [Obsolete("This is going away, use AnnualPrice instead")]
        public decimal? Price { get; set; }

        /// <summary>
        /// The annual subscription fee.
        /// </summary>
        public decimal? AnnualPrice { get; set; }

        /// <summary>
        /// Verbose prose for plan description
        /// </summary>
        public string Description { get; set; }

    }
}



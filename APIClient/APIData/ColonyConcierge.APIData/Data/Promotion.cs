using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class Promotion
    {
        public int ID { get; set; }

        public bool IsPublished { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string DetailedDescription { get; set; }

        public string PromoCode { get; set; }

        public decimal? TotalMonitaryQuota { get; set; }

        public int? TotalUseCountQuota { get; set; }

        public int? PerUserUseCountQuota { get; set; }

        public TimeStamp StartsOn { get; set; }

        public TimeStamp ExpiresOn { get; set; }

        /// <summary>
        /// If this promotion contains *both* a flat amount and a percentage amount
        /// The this flat amount should be interpreted as a maximum discount amount.
        /// </summary>
        public decimal? DiscountFlatAmount { get; set; }

        public decimal? DiscountPercentageAmount { get; set; }

        public bool LimitToFees { get; set; }

        public bool DiscountAsCash { get; set; }

        /// <summary>
        /// If this is true, then this discount may not be combined with other offers. 
        /// </summary>
        public bool IsExclusive { get; set; }

        /// <summary>
        /// If this is true, then this discount should not *count* when other discounts are marked exclusive. 
        /// This could be used for things like "store credit", where a customer could still use an "exclusive" promotion and a store credit at the same time.
        /// </summary>
        public bool AlwaysAllowUse { get; set; }

        public List<int> ServiceIDs { get; set; }

        public List<int> DestinationIDs { get; set; }

    }
}

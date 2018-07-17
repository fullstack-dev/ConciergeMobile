using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This class can be used be clients to know about how a Promotion or discount might be applied to a pending order.
    /// </summary>
    public class PendingDiscount
    {
        public string Name { get; set; }

        /// <summary>
        /// The name that can be shown by the front end
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A more in-depth description prose
        /// </summary>
        public string DetailedDescription { get; set; }

        /// <summary>
        /// The number times a given user may use this promotion (total quota)
        /// </summary>
        public int? PerUserUseCountQuota { get; set; }

        /// <summary>
        /// The promotion may not be used for orders placed prior to this timestamp
        /// </summary>
        public TimeStamp StartsOn { get; set; }

        /// <summary>
        /// The promotion may not be used for orders placed after this timestamp
        /// </summary>
        public TimeStamp ExpiresOn { get; set; }

        /// <summary>
        /// If this discount represents a flat amount (eg "$1.00 off"), then this amount will be populated.
        /// If a <see cref="DiscountPercentageAmount"/> is also specified, then this value serves as a maximum dollar amount.
        /// </summary>
        public decimal? DiscountFlatAmount { get; set; }

        /// <summary>
        /// If this discount represents a percentage discount amount, then this will be that percentage (expresses as a range 1.0 - 0.0).
        /// If a <see cref="DiscountFlatAmount"/> is also specified, then that value serves as a maximum dollar amount for the discount.
        /// </summary>
        public decimal? DiscountPercentageAmount { get; set; }

        /// <summary>
        /// If true, then the discount should apply only to fees on the order, not to any subtotal
        /// </summary>
        public bool LimitToFees { get; set; }

        /// <summary>
        /// This impacts how taxes are computed. If this is 'true' then the discount should be treated as though it were cash,and the sales taxes should be applied
        /// to the amount before the discount is applied. If this is false, then this discount is treated as a "reduction in retail price", and the sales
        /// tax should be computed after the discount is applied. Different jurisdictions have different rules for this.
        /// REgardless of this setting, percentage based discounts always work with the "pre-tax" amounts to compute the discount value. 
        /// </summary>
        public bool DiscountAsCash { get; set; }


        /// <summary>
        /// If this is true, then this discount may not be combined with other offers. 
        /// </summary>
        [Obsolete("NOT IMPLEMENETED YET")]
        public bool IsExclusive { get; set; }


        /// <summary>
        /// If this is true, then this discount should not *count* when other discounts are marked exclusive. 
        /// This could be used for things like "store credit", where a customer could still use an "exclusive" promotion and a store credit at the same time.
        /// </summary>
        [Obsolete("NOT IMPLEMENETED YET")]
        public bool AlwaysAllowUse { get; set; }

        /// <summary>
        /// If this list is empty, then there is not restriction on what types of services this discount may be used for. Otherwise this is a list of 
        /// services ID's to which this discount is limited.
        /// </summary>
        public List<int> ServiceIDs { get; set; }

        /// <summary>
        /// If this list is empty, then there is not restriction on which destinations this discount may be used for. Otherwise this is a list of 
        /// destination ID's to which this discount is limited.
        /// </summary>
        public List<int> DestinationIDs { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents a Coupon object that can be applied to a specific order by a specific user.
    /// </summary>
    public class Coupon
    {

        public int ID { get; set; }

        /// <summary>
        /// If this coupon has an expiration, this will be non-null
        /// </summary>
        public DateTime? Expiration { get; set; }


        /// <summary>
        /// If this coupon represents a fixed, "flat amount" discount, then this value will be non-null
        /// </summary>
        public decimal? FlatAmount { get; set; }


        /// <summary>
        /// If this coupon represents a percentage discount, then this value will be non null, on the range of 0-1 for discount percentage.
        /// </summary>
        public decimal? PercentageOffAmount { get; set; }


        /// <summary>
        /// If this coupon has a cap on the allowed amount, then this value will be non-null
        /// </summary>
        public decimal? MaxAmount { get; set; }


        /// <summary>
        /// If this coupon is tied to a specific user account (meaning it is only usable by that user, or that it has already been used by that user), then this value will 
        /// be nonzero
        /// </summary>
        public int BoundUserID { get; set; }


        /// <summary>
        /// If the coupon has already been used, then this will be set
        /// </summary>
        public bool Used { get; set; } 
        

    }
}

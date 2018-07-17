using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Base class for payment methods
    /// </summary>
    public class PaymentAccountData : InheritedTypeBase
    {

        /// <summary>
        /// The Payment ID for this payment method. This is read-only, set on the server.
        /// </summary>
        public int ID { get; set; }


        /// <summary>
        /// User-visible name on the account payment method.
        /// </summary>
        public string AccountNickname { get; set; }

        /// <summary>
        /// The billing name for the payment method
        /// </summary>
        public string NameOnAccount { get; set; }

        /// <summary>
        /// Set to 'null' to use service address
        /// </summary>
        public Address BillingAddress { get; set; }

        ///// <summary>
        ///// An arbitrary number, generally 1-100 used to order relative importance of accounts. Higher values are returned first in lists.
        ///// The internal default priority if not specified is "50"
        ///// </summary>
        //public int? Priority { get; set; }

        /// <summary>
        /// Sets if the payment method should be considered "preferred"
        /// If this is set to "true" on any add payment method or update, all other payment methods on the account will have the flag cleared.
        /// </summary>
        public bool IsPreferred { get; set; }

        public string PaymentNonce { get; set; }

    }
}

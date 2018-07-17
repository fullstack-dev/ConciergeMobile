using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Credit card information used on an account for payment processing.
    /// </summary>
    public class CreditCardData : PaymentAccountData
    {
        /// <summary>
        /// The credit card number, as it appears on the front of the card
        /// </summary>
        public string CreditCardNumber { get; set; }

        /// <summary>
        /// The "security code" printed on the card
        /// </summary>
        [Obsolete("This field is never used by the server.")]
        public string SecurityCode { get; set; }

        /// <summary>
        /// The month (1-12) of the expiration of the card.
        /// </summary>
        public int ExpirationMonth { get; set; }

        /// <summary>
        /// The year, in four digit form, of the expiration of the credit card
        /// </summary>
        public int ExpirationYear { get; set; }

        /// <summary>
        /// Set to true if this is a Debit Card, false for Credit Card
        /// </summary>
        public bool IsDebit { get; set; }

        public string creditType { get; set; }

        public string creditTypeImg { get; set; }

        public bool isExpired { get; set; }

        public string Token { get; set; }

    }
}

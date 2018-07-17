using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class to hold information about payments
    /// </summary>
    public class Payment
    {
        public int ID { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// The date/time that the payment took place.
        /// </summary>
        public TimeStamp PaymentTime { get; set; }


        /// <summary>
        /// The related invoice ID
        /// </summary>
        public int InvoiceID { get; set; }

        /// <summary>
        /// The user that is associated with this payment.
        /// </summary>
        public int RelatedUser { get; set; }


        /// <summary>
        /// The id of the payment method used by the user
        /// </summary>
        public int PaymentMethodID { get; set; }
    }
}

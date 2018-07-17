using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledService : InheritedTypeBase
    {

        public int ID { get; set; }

        /// <summary>
        /// The "Display Name" of this service.
        /// <note type="note">This is "read-only" in that the server ignore incoming values, and will set the value 
        /// on outgoing objects automatically.</note>
        /// </summary>
        public string Name { get; set; }


        public bool IsEmergency { get; set; }

        /// <summary>
        /// The time/date that this service was entered. 
        /// This is set automatically by the server.
        /// </summary>
        public TimeStamp EntryDate { get; set; }

        /// <summary>
        /// The date which this service is to be performed.
        /// </summary>
        public SimpleDate ServiceDate { get; set; }

        /// <summary>
        /// For some order types, this allows the client side to specify that this is a special destination. 
        /// If this option is specified, the Service Address value is not needed.
        /// </summary>
        public int DestinationID { get; set; }


        /// <summary>
        /// The time at the start of the time frame at which the service is to be performed, if applicable for this service, or null, otherwise.
        /// <note>This time is READ ONLY, sent from the server to clients as a convenience, it is not a settable property on the service!</note>
        /// </summary>

        public TimeStamp ServiceStartTime { get; set; }


        /// <summary>
        /// The time at the end of the time frame at which the service is to be performed, if applicable for this service, or null, otherwise.
        /// <note>This time is READ ONLY, sent from the server to clients as a convenience, it is not a settable property on the service!</note>
        /// </summary>
        public TimeStamp ServiceEndTime { get; set; }

        public ExtendedAddress ServiceAddress { get; set; }

        /// <summary>
        /// The payment method that should be used to pay for this service.
        /// This is currently optional, and the server will attempt to use the user's "Preferred Payment method" is this field is 0.
        /// </summary>
        public int PaymentMethodID { get; set; }

        /// <summary>
        /// The customer-supplied tip
        /// </summary>
        public decimal Tip { get; set; }

        public string Status { get; set; }

        public string SpecialInstructions { get; set; }

        public int RelatedUserID { get; set; }

        /// <summary>
        /// A comma-separated list of discount codes to apply.
        /// If a particular code is to be applied more than once, it should appear twice in the list.
        /// </summary>
        public string DiscountCodes { get; set; }

        /// <summary>
        /// The invoice ID associated with this service. 
        /// This may be '0', if this service does not have an invoice yet.
        /// </summary>
        public int InvoiceID { get; set; }


        /// <summary>
        /// This is the ID of the assigned service definition
        /// This is required to place an order.
        /// </summary>
        public int ServiceID { get; set; }



    }
}

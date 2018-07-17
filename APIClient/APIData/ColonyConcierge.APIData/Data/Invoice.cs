using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class that represents an invoice on an account.
    /// </summary>
    public class Invoice
    {
        public int ID { get; set; }

        /// <summary>
        /// A description of the invoice (max 128 characters)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The date/time that the invoice was created. This is a 'readonly' property set by the server
        /// </summary>
        public virtual TimeStamp InvoiceDate { get; set; }


        /// <summary>
        /// Line item charges for the invoice
        /// </summary>
        public List<InvoiceLineItem> LineItems { get; set; }


        /// <summary>
        /// The service fee for the invoice.
        /// </summary>
        /// <remarks>
        /// Note that this is for reference only. The <see cref="LineItems"/> collection will contain an item for this fee.
        /// This is to make it easier to generate reports on the fee that was charge, but it is not used for 
        /// computing the amount due.
        /// </remarks>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// The customer-supplied tip
        /// </summary>
        /// <remarks>
        /// Note that this is for reference only. The <see cref="LineItems"/> collection will contain an item for this amount.
        /// This is to make it easier to generate reports on the tip that was specified, but it is not used for 
        /// computing the amount due.
        /// </remarks>
        public decimal Tip { get; set; }

        /// <summary>
        /// Represents the dollar amount of the sum of the discounts applied on this invoice.
        /// </summary>
        /// <remarks>
        /// Note that this is for reference only. The <see cref="LineItems"/> collection will contain an item for this amount.
        /// This is to make it easier to generate reports on the discount that was specified, but it is not used for 
        /// computing the amount due.
        /// </remarks>
        public decimal Discounts { get; set; }

        /// <summary>
        /// Represents the dollar amount of the sum of the discounts applied on this invoice.
        /// </summary>
        /// <remarks>
        /// Note that this is for reference only. The <see cref="LineItems"/> collection will contain an item for this amount.
        /// This is to make it easier to generate reports on the discount that was specified, but it is not used for 
        /// computing the amount due.
        /// </remarks>

        public decimal Tax { get; set; }


        /// <summary>
        /// Flag indicating if the invoice has been paid
        /// </summary>
        public bool Paid { get; set; }


        /// <summary>
        /// The IDs of the payment(s) for this invoice, if it has been paid.
        /// </summary>
        public List<int> PaymentIDs { get; set; }

        /// <summary>
        /// The total charges for this invoice.
        /// This a calculated, 'Read only' property that cannot be set by clients.
        /// </summary>
        public decimal TotalDue { get; set; }

        public Invoice()
        {
            LineItems = new List<InvoiceLineItem>();
            PaymentIDs = new List<int>();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// A single line item charge in an invoice <see cref="Invoice">invoice</see>
    /// </summary>
    public class InvoiceLineItem
    {
        /// <summary>
        /// The ID of this line item
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// An explanation of what this charge is for
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The amount, not including tax, of this line item
        /// </summary>
        public decimal BaseAmount { get; set; }

        /// <summary>
        /// The amount of tax associated with this item
        /// </summary>
        public decimal TaxAmount { get; set; }
    }
}

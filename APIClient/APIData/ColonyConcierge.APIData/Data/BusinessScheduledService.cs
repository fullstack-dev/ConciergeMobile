using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This class allows for businesses to retrieve scheduled services with extra related metadata
    /// </summary>
    public class BusinessScheduledService
    {
        public List<int> JobIDs { get; set; }

        public int AssignedWorkerID { get; set; }

        public string AssignedWorkerDisplayName { get; set; }

        public ScheduledService WrappedScheduledService { get; set; }

        /// <summary>
        /// The total amount of the invoice, if one exists
        /// </summary>
        public decimal InvoiceTotal { get; set; }

        public decimal InvoiceID { get; set; }

        /// <summary>
        /// True if the order is considered "paid"
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// True, if applicable on the order has been reported "delivered" by logistics.
        /// </summary>
        public bool IsDelivered { get; set; }

        public BusinessScheduledService()
        {
            JobIDs = new List<int>();
        }

    }
}

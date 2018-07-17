using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledServiceDispatchAssignment
    {
        public int ID { get; set; }

        /// <summary>
        /// This is the ID of the service that this driver is to complete.
        /// </summary>
        public int ScheduledServiceID { get; set; }

        /// <summary>
        /// This is the ID of the contractor or delivery driver assigned to complete this task
        /// </summary>
        public int AssignedUserID { get; set; }
    }
}

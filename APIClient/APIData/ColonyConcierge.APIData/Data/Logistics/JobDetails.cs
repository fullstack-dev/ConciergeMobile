using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class JobDetails
    {

        public int ID { get; set; }

        /// <summary>
        /// This order# associated with this job, if any
        /// </summary>
        public int OrderID { get; set; }


        /// <summary>
        /// The name of the end point assocated with this job. 
        /// This could be a restaurant or grocery store name, or the name of a delivery customer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The addresses associated with this job.
        /// </summary>
        public List<ExtendedAddress> Addresses { get; set; }


        /// <summary>
        /// The phone numbers associated with this job.
        /// </summary>
        public List<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// Generic line item details for this job
        /// </summary>
        public List<JobDetailLineItem> DetailLines { get; set; }
    }
}

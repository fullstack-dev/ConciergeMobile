using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    /// <summary>
    /// Data related to and specific to operations performed by job managers.
    /// </summary>
    public class JobManagementData
    {
        public Job Job { get; set; }

        /// <summary>
        /// Don't rely on this for semantic meaning! It could change on the server side!
        /// </summary>
        public string Status { get; set; }

        public bool IsAutoOfferEnabled {get; set; }

        public User AssignedWorker { get; set; }

        
    }
}

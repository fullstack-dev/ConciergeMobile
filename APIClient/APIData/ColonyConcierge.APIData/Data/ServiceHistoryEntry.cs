using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class that represents a history item on a schedule service, for tracking changes to the service over time.
    /// These entries are created on the server in response to certain operations and APIs being called on services. 
    /// Client code cannot directly create them. (see the IScheduledServices API for adding user and admin notes for that)
    /// </summary>
    public class ServiceHistoryEntry
    {
        public int ID { get; set; }

        public int InitiatingUserID { get; set; }

        public TimeStamp EntryTime { get; set; }

        public string Comment { get; set; }

        public bool CustomerVisisble { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class WorkerRegistrationData
    {

        public NotificationRecipientInformation NotificationInfo { get; set; }

        /// <summary>
        /// Should be 'true' if the worker is an actual employee, and not just a contractor
        /// </summary>
        public bool IsEmployee { get; set; }
    }
}

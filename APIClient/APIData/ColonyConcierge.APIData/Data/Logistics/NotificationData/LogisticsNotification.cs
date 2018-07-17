using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics.NotificationData
{
    public class LogisticsNotification
    {

        public NotificationKind Kind { get; set; }

        /// <summary>
        /// Keep this short!
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A list of ID's related to this message. 
        /// The meaning of the ID's will depend on the kind of notification, and might be empty for some kinds
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// If applicable, the latitude related to this message, otherwise, null
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// If applicable, the longitude related to this message, otherwise, null
        /// </summary>
        public double? Long { get; set; }
        
    }
}

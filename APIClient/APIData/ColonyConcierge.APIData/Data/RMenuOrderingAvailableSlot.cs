using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{

    [DebuggerDisplay("ID = {ID}, Loc = {RelatedLocationID} S = {StartTime}, E = {EndTime}, Delivery = {IsDelivery}")]
    public class RMenuOrderingAvailableSlot
    {
        public int ID { get; set; }


        public int RelatedLocationID { get; set; }


        /// <summary>
        /// This is a readonly property added as a convienience by the server.
        /// </summary>
        public List<int> RelatedMenuIDs { get; set; }

        /// <summary>
        /// This is the "beginning" of the delivery/pickup window
        /// </summary>
        public TimeStamp StartTime { get; set; }

        /// <summary>
        /// This is the end of the delivery/pickup window.
        /// </summary>
        public TimeStamp EndTime { get; set; }

        public bool IsDelivery { get; set; }

        /// <summary>
        /// How soon before the <see cref="StartTime"/> an order must be placed. 
        /// </summary>
        public int LeadTimeMinutes { get; set; }

        /// <summary>
        /// This is the time by which the order must be placed. This is computed as <see cref="LeadTimeMinutes"/> minute before the <see cref="StartTime"/>
        /// </summary>
        public TimeStamp CutoffTime { get; set; }

        /// <summary>
        /// Represents how many orders can be placed in this slot. "null" indicates that there is no limit.
        /// </summary>
        public int? RemainingCount { get; set; }
    }
}

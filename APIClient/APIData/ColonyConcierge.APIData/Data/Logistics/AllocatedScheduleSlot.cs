using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{

    public class AllocatedScheduleSlot
    {
        public int ID { get; set; }

        /// <summary>
        /// Read only convenience property coming from the server
        /// </summary>
        public int RelatedScheduleSlotID { get; set; }

        /// <summary>
        /// Read only property coming from the server.
        /// </summary>
        public int RelatedScheduleGroupID { get; set; }


        public string Name { get; set; }

        public string DisplayName { get; set; }
        
        public TimeStamp StartTime { get; set; }

        public TimeStamp EndTime { get; set; }

        public int TotalQuota { get; set; }

        /// <summary>
        /// This is a READ ONLY property, calculated dynamically on the server at the time of the call!
        /// 
        /// </summary>
        public int RemainingQuota { get; set; }

        /// <summary>
        /// This is a READ ONLY property, optionally created on certain user-context APIs.
        /// This is "true", if the user is signed up for this slot (either under the quota, or on standby), false is not signed up, or null, if not provided.
        /// </summary>
        public bool? SignedUp { get; set; }

        /// <summary>
        /// This is a READ ONLY property, optionally created on certain user-context APIs
        /// This is "true", if the user's signup for this slot is a standby signup, false if it is under qouta, or 'null', if not signed up/unknown 
        /// </summary>
        public bool? SignupIsStandby { get; set; }




    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class WorkerClockInInfo
    {
        public bool IsClockedIn { get; set; }
        public TimeStamp LastClockIn { get; set; }
        public TimeStamp LastClockOut { get; set; }
        public bool CanClockInNow { get; set; }
        /// <summary>
        /// Set to 'true' if the user is currently clocked out, and the last clockout was a force-clockout
        /// </summary>
        public bool WasForcedClockedOut { get; set; }
        /// <summary>
        /// The time at which this user will be allowed to clock in again.
        /// </summary>
        public TimeStamp NextAllowedClockInTime { get; set; }

        /// <summary>
        /// If the user has signed for for any active time slots, then this will be populated.
        /// </summary>
        public TimeStamp NextActiveScheduleTime { get; set; }

        /// <summary>
        /// I the user has signed up for any standby timeslots, then this will be activated.
        /// </summary>
        public TimeStamp NextStandbyScheduleTime { get; set; }
    }
}

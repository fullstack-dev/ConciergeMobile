using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class ScheduleSlot
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// Timezone-agnostic time of day
        /// </summary>
        public TimeOfDay StartTime { get; set; }

        /// <summary>
        /// Timezone-agnostic time of day for the end of the time slot.
        /// </summary>
        public TimeOfDay EndTime { get; set; }

        /// <summary>
        /// Day of week for this slot. 0 = Sunday, 1 = Monday, etc.
        /// </summary>
        public int DayOfWeek { get; set; }


        public int Quota { get; set; }
    }
}

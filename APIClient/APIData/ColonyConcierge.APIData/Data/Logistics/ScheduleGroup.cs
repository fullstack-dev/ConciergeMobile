using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class ScheduleGroup
    {
        public int ID { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The number of days that a planning scheduler sohuld consider at a time, when creating allocated schedule slots.
        /// </summary>
        public int PlanAheadDays { get; set; }

        /// <summary>
        /// The "cron spec" type string that defines when the scheduler should run to allocate schedule slots.
        /// </summary>
        public string PlanningSchedule { get; set; }

        public string TimeZone { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class ScheduleGroupHoliday
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// The date. (non-timezone, should be in local time of restaurant)
        /// </summary>
        public SimpleDate Date { get; set; }


        /// <summary>
        /// The start time for the holiday (non-timezone, should be in local time of restaurant)
        /// </summary>
        public TimeOfDay StartTime { get; set; }

        /// <summary>
        /// The end time for the holiday. (non-timezone, should be in local time of restaurant)
        /// </summary>
        public TimeOfDay EndTime { get; set; }

    }
}

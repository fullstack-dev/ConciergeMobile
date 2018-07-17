﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ServiceCalendar
    {
        public ServiceDaysOfWeek DaysOfWeekFilter { get; set; }

        public IList<ServiceHolidayDate> Holidays { get; set; }

    }
}

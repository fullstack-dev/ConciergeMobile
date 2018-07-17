using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Charges related to personal services.
    /// </summary>
    public class ScheduledPersonalServiceCharges : ScheduledServiceCharges
    {
        /// <summary>
        /// The normal rate for laundry charges.
        /// </summary>
        public decimal? RegularLaundryRate { get; set; }

        /// <summary>
        /// The number of pounds of laundry processed.
        /// </summary>
        public int? RegularLaundryPounds { get; set; }

        /// <summary>
        /// The rate for heavily soiled laundry 
        /// </summary>
        public decimal? HeavyLaundryRate { get; set; }

        /// <summary>
        /// The number of pounds of heavily soiled laundry processed.
        /// </summary>
        public int? HeavyLaundryPounds { get; set; }

        /// <summary>
        /// The hourly rate applied for waiting
        /// </summary>
        public decimal? WaitingRate { get; set; }

        /// <summary>
        /// The number of hours of waiting as part of this charge
        /// </summary>
        public double? WaitingHours { get; set; }

        /// <summary>
        /// Charges related to Handyman services, if any
        /// </summary>
        public List<HandymanCharge> HandymanCharges { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledErrandCharges : ScheduledServiceCharges
    {
        /// <summary>
        /// The amount, if any, to pass on to the customer for dry cleaning charges.
        /// </summary>
        public decimal? DryCleaningCharge { get; set; }
    }
}

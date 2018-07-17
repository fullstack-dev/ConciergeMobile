using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledSpecialRequestCharges : ScheduledServiceCharges
    {
        public List<ScheduledSpecialRequestChargeItem> ChargeItems { get; set; }

        public ScheduledSpecialRequestCharges()
        {
            ChargeItems = new List<ScheduledSpecialRequestChargeItem>();
        }

    }
}

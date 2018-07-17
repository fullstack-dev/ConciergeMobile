using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledShoppingCharges : ScheduledServiceCharges
    {
        public List<ScheduledShoppingChargeItem> ChargeItems { get; set; }


        public ScheduledShoppingCharges()
        {
            ChargeItems = new List<ScheduledShoppingChargeItem>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledRestaurantServiceCharges : ScheduledServiceCharges
    {
        public List<ScheduledRestaurantServiceChargeItem> ChargeItems { get; set; }


        public ScheduledRestaurantServiceCharges()
        {
            ChargeItems = new List<ScheduledRestaurantServiceChargeItem>();
        }

    }
}

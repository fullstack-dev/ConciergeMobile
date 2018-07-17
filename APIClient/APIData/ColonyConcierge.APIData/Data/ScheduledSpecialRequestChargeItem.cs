using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledSpecialRequestChargeItem
    {
        public int ID { get; set; }

        public string Description { get; set; }

        public decimal Charge { get; set; }

        public DateTime? Time { get; set; }
    }
}

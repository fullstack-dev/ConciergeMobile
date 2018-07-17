using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledRMenuItemComboSelection
    {
        public int ID { get; set; }

        //public int Quantity { get; set; }

        public ScheduledRMenuItem MenuItem { get; set; }

        public int RelatedSlotID { get; set; }


    }
}

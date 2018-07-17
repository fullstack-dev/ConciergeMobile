using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class RMenuItemCombinationData
    {
        public int ID { get; set; }

        public List<RMenuItemCombinationSlot> Slots { get; set; }
    }
}

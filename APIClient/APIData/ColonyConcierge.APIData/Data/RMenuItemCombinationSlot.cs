using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class RMenuItemCombinationSlot
    {
        public int ID { get; set; }

        /// <summary>
        /// Defines the minimum number of menu items that must be a part of this slot. 
        /// </summary>
        public int MinApplied { get; set; }

        /// <summary>
        /// Defines the maximum number of menu items that can be a part of this slot.
        /// </summary>
        public int MaxApplied { get; set; }


        public List<int> TagIDs { get; set; }
    }
}

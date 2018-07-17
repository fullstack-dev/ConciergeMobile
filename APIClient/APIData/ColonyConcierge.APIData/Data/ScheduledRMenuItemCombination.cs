using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledRMenuItemCombination
    {
        public int ID { get; set; }

        public List<ScheduledRMenuItemComboSelection> SlotSelections { get; set; }
        
        public ScheduledRMenuItemCombination()
        {
            SlotSelections = new List<ScheduledRMenuItemComboSelection>();
        } 
    }
}

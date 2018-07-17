using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledRMenuItem
    {
        public int ID { get; set; }

        public int Quantity { get; set; }

        public int RelatedMenuID { get; set; }

        public int RelatedMenuItemID { get; set; }

        /// <summary>
        /// This is set automatically by the server at order creation time. Results should come sorted in order of this priority, so you should need to sort 
        /// it yourself
        /// </summary>
        public int Priority { get; set; }

        public string DisplayName { get; set; }

        public string SpecialInstructions { get; set; }

        /// <summary>
        /// This property is used only for validation when placing or editing an order. 
        /// final price is calculated server side.
        /// The value that is stored with the order is computed from the server side logic, not that value that was passed in (though they ought to match)
        /// </summary>
        [Obsolete("This property is no longer used on the server side.")]
        public decimal FrontEndPrice { get; set; }

        /// <summary>
        /// This Property is set on the server, based on menu data at the time the order is placed.
        /// </summary>
        public bool IsRetail { get; set; }

        public ScheduledRMenuItemCombination Combination { get; set; }

        public IList<ScheduledRMenuItemModifier> AppliedModifiers { get; set; }


        public ScheduledRMenuItem()
        {
            AppliedModifiers = new List<ScheduledRMenuItemModifier>();
        }
    }
}

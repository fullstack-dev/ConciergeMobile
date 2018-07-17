using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents an *Application* of a modifier to a menu item
    /// Care must be taken to ensure that the modifer is legal to apply to the menuitem.
    /// The server will reject orders that have invalid modifers. 
    /// (Though early in development, not all validations may be applied yet)
    /// </summary>
    public class ScheduledRMenuItemModifier
    {
        public int ID { get; set; }

        //The quantity of this modifier to apply to the menu item
        public int Quantity { get; set; }

        //The ID of the modifier
        public int RelatedModiferID { get; set; }

        /// <summary>
        /// This is set automatically by the server at order creation time. Results should come sorted in order of this priority, so you should not need to sort in general.
        /// However, the front end has a slightly different paradigm for what constitutes how modifiers are ordered and displayed. The API Server has no 
        /// concept of "Add" or "Remove" for modifiers, only a quantity for the modifier that specifies how many of that modifier to use. If a client wants to place 
        /// "remove" modifiers first, it needs to adjust the sorting on it's end accordingly.
        /// 
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// If this modifier was a submodifier enabled by selecting some other modifier, then this will be set.
        /// It is important to remember the MODIFERS ALWAYS MODIFY MENU ITEMS, NOT OTHER MODIFERS EVEN IF THEY ARE A 'SUBMODIFIER'.
        /// Therefore, this property is somewhat of a hack in that it is created by the server attempting to "guess" what the intended "parent" modifier was by
        /// examining the menu data at the time that the order is placed.
        /// </summary>
        public int EstimatedParentModifer { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// This is set automatically by the server at order creation time.
        /// </summary>
        public bool AppliedByDefault { get; set; }
    }
}

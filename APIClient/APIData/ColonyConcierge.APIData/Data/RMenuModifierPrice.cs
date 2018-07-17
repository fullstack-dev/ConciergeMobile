using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This data is for when modifiers apply a price change to the item they are applied to in a way that is specific to that
    /// combination if menu item and modifier.
    /// </summary>
    public class RMenuModifierPrice
    {
        public int ID { get; set; }

        /// <summary>
        /// The price of the modification.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// This is 'true' when this modifier price should be "additive" to the base price. Otherwise, this overrides the base price.
        /// </summary>
        public bool Additive { get; set; }

        /// <summary>
        /// This id of the modifier. This must be a modifier that is in a group that is already applied to this menu item.
        /// </summary>
        public int ModifierID { get; set; }
    }
}

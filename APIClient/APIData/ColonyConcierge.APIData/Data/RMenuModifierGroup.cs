using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// ModifierGroups are collections of <see cref="RMenuModifier"/> objects that may be 
    /// reference by a menu item. Modifiers themselves may also reference a modifier group 
    /// which has the effect that modifiers in that group would then also be available to the menu item if 
    /// the parent modifier is selected.
    /// </summary>
    [DebuggerDisplay("ID = {ID}, Name = {Name} DisplayName = {DisplayName}")]

    public class RMenuModifierGroup
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Defines the minimum number of modifiers that must be applied on an item using this group. Unbounded if null
        /// </summary>
        public int? MinApplied { get; set; }

        /// <summary>
        /// Defines the maximum number of modifiers that must be applied on an item in this group. Unbounded if null
        /// </summary>
        public int? MaxApplied { get; set; }

        public List<int> ModifierIDs { get; set; }

    }
}

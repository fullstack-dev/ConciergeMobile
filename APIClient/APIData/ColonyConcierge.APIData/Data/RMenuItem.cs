using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Data related to a menu item on a restaurants menu
    /// </summary>
    [DebuggerDisplay("ID = {ID}, Name = {Name} DisplayName = {DisplayName}")]
    public class RMenuItem
    {
        public int ID { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string DetailedDescription { get; set; }

        public decimal? BasePrice { get; set; }

        /// <summary>
        /// Optional tax rate for this item that overrides the restaurant location's default tax rate.
        /// </summary>
        public decimal? TaxRate { get; set; }

        /// <summary>
        /// Potential additional order lead time needed for this menu item
        /// </summary>
        public int? OrderLeadOverride { get; set; }


        /// <summary>
        /// "true" if this item should only appear as part of a combination
        /// </summary>
        public bool ComboOnly { get; set; }

        /// <summary>
        /// Contains information related to combination options for this menu item, if this menu item is a combination(eg it references other menu items)
        /// </summary>
        public RMenuItemCombinationData CombinationData { get; set; }


        /// <summary>
        /// If this is "true", then this item is supplied by AsystYou, and not the restaurant. Therefore, this item will not be allowed on pickup orders.
        /// </summary>
        public bool IsRetail { get; set; }

        /// <summary>
        /// If this is non-null, then this item has a per-order max quantity limit.
        /// </summary>
        public int? MaxQuantity { get; set; }


        public int ParentGroupID { get; set; }


        /// <summary>
        /// Includes *all* tags associated with this menu item,
        /// </summary>
        public List<int> TagIDs { get; set; }

        /// <summary>
        /// List of tag IDs on this menuitem which have the IsCategory flag set.
        /// </summary>
        /// <remarks>
        /// <note type="note">This is a READ ONLY property, computed on the server, you cannot set this. The list is determined by the set of categories
        /// on this menuitem that have the "<see cref="RMenuTag.IsCategory"/> flag set to "true". </note>
        /// </remarks>
        public List<int> CategoryTagIDs { get; set; }

        public List<int> ModifierGroupIDs { get; set; }

        public List<int> ModifierPriceIDs { get; set; }



    }
}

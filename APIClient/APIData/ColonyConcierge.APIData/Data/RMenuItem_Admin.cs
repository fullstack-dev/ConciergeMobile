using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ColonyConcierge.APIData.Data
{
    [DataContract]
    public class RMenuItem_Admin
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool IsPublished { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string DetailedDescription { get; set; }

        [DataMember]
        public decimal? BasePrice { get; set; }

        /// <summary>
        /// Optional tax rate for this item that overrides the restaurant location's default tax rate.
        /// </summary>
        [DataMember]
        public decimal? TaxRate { get; set; }

        /// <summary>
        /// Potential additional order lead time needed for this menu item
        /// </summary>
        [DataMember]
        public int? OrderLeadOverride { get; set; }

        /// <summary>
        /// "true" if this item should only appear as part of a combination
        /// </summary>
        [DataMember]
        public bool ComboOnly { get; set; }

        /// <summary>
        /// Contains information related to combination options for this menu item, if this menu item is a combination(eg it references other menu items)
        /// </summary>
        [DataMember]
        public RMenuItemCombinationData CombinationData { get; set; }

        [DataMember]
        public int ParentGroupID { get; set; }

        [DataMember]
        public int Priority { get; set; }


        /// <summary>
        /// Includes *all* tags associated with this menu item,
        /// </summary>
        [DataMember]
        public List<int> TagIDs { get; set; }

        /// <summary>
        /// List of tag IDs on this menuitem which have the IsCategory flag set.
        /// </summary>
        /// <remarks>
        /// <note type="note">This is a READ ONLY property, computed on the server, you cannot set this. The list is determined by the set of categories
        /// on this menuitem that have the "<see cref="RMenuTag.IsCategory"/> flag set to "true". </note>
        /// </remarks>
        [DataMember]
        public List<int> CategoryTagIDs { get; set; }

        [DataMember]
        public List<int> ModifierGroupIDs { get; set; }

        [DataMember]
        public List<int> ModifierPriceIDs { get; set; }
    }
}

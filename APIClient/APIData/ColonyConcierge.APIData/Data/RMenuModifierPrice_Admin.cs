using System.Runtime.Serialization;

namespace ColonyConcierge.APIData.Data
{
    [DataContract]
    public class RMenuModifierPrice_Admin
    {
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// The price of the modification.
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// This is 'true' when this modifier price should be "additive" to the base price. Otherwise, this overrides the base price.
        /// </summary>
        [DataMember]
        public bool Additive { get; set; }

        /// <summary>
        /// This id of the modifier. This must be a modifier that is in a group that is already applied to this menu item.
        /// </summary>
        [DataMember]
        public int ModifierID { get; set; }

        [DataMember]
        public int MenuItemID { get; set; }
    }
}

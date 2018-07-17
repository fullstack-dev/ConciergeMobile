using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ColonyConcierge.APIData.Data
{
    [DataContract]
    public class RMenuModifierGroup_Admin
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool IsPublished { get; set; }

        /// <summary>
        /// Defines the minimum number of modifiers that must be applied on an item using this group. Unbounded if null
        /// </summary>
        [DataMember]
        public int? MinApplied { get; set; }

        /// <summary>
        /// Defines the maximum number of modifiers that must be applied on an item in this group. Unbounded if null
        /// </summary>
        [DataMember]
        public int? MaxApplied { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public List<int> ModifierIDs { get; set; }
    }
}

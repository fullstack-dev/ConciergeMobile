using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ColonyConcierge.APIData.Data
{
    [DataContract]
    public class RMenuModifier_Admin
    {
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Represents a simple name for the modifier. 
        /// This does not need to be unique system wide, but ought to be unique within a particular menu
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// A displayable string to be used in user interfaces. 
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool IsPublished { get; set; }

        /// <summary>
        /// An <em>optional</em> price that might apply if this modifier is selected. 
        /// </summary>
        /// <remarks>
        /// CAUTION: Pricing is a very complex topic in the restaurant service API. This is only one of many data pieces that may apply/contribute
        /// to the final price.
        /// </remarks>
        [DataMember]
        public decimal? Price { get; set; }

        [DataMember]
        public decimal OverridePrice { get; set; }

        [DataMember]
        public int OverridePriceID { get; set; }

        [DataMember]
        public bool Additive { get; set; }

        [DataMember]
        public int ParentGroupID { get; set; }

        /// <summary>
        /// A more descriptive, lengthy text, describing the modifier.
        /// </summary>
        [DataMember]
        public string DetailedDescription { get; set; }

        /// <summary>
        ///  This indicates if this modifier should be applied by default. 
        ///  This is an indicator to front ends. The API server logic will not apply this automatically.
        /// </summary>
        [DataMember]
        public bool ApplyByDefault { get; set; }

        /// <summary>
        /// This indicates if the modifier can be applied more than once to an item.
        /// </summary>
        [DataMember]
        public bool AllowMultipleInstances { get; set; }

        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// This is a list of ID's of submodfier groups that can be applied if this modifier is active.
        /// </summary>
        [DataMember]
        public List<int> SubmodifierGroupIDs { get; set; }
    }
}

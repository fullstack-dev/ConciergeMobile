using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents a selection of a modifier on a particular menu item.
    /// </summary>
    [DebuggerDisplay("ID = {ID}, Name = {Name} DisplayName = {DisplayName}")]
    public class RMenuModifier
    {
        public int ID { get; set; }

        /// <summary>
        /// Represents a simple name for the modifier. 
        /// This does not need to be unique system wide, but ought to be unique within a particular menu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A displayable string to be used in user interfaces. 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// An <em>optional</em> price that might apply if this modifier is selected. 
        /// </summary>
        /// <remarks>
        /// CAUTION: Pricing is a very complex topic in the restaurant service API. This is only one of many data pieces that may apply/contribute
        /// to the final price. In addition, this part o the data model is in early development, and is subject to change.
        /// </remarks>
        public decimal? Price { get; set; }
        

        /// <summary>
        /// A more descriptive, lengthy text, describing the modifier.
        /// </summary>
        public string DetailedDescription { get; set; }

        /// <summary>
        ///  This indicates if this modifier should be applied by default. 
        ///  This is an indicator to front ends. The API server logic will not apply this automatically.
        /// </summary>
        public bool ApplyByDefault { get; set; }


        /// <summary>
        /// This indicates if the modifier can be applied more than once to an item.
        /// </summary>
        public bool AllowMultipleInstances { get; set;  }

        /// <summary>
        /// This is a list of ID's of submodfier groups that can be applied if this modifier is active.
        /// </summary>
        public List<int> SubmodifierGroupIDs { get; set; }

    }
}

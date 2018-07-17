using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents a "group" within a restaurant menu to which menu items or other groups may belong
    /// </summary>
    public class RMenuGroup
    {
        public int ID { get; set; }

        /// <summary>
        /// This group id of the parent group, iff this group has a parent
        /// </summary>
        public int ParentID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }


        public string DetailedDescription { get; set; }



    }
}

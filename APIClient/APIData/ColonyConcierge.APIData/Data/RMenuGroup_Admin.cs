using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class RMenuGroup_Admin
    {
        public int ID { get; set; }

        /// <summary>
        /// This group id of the parent group, iff this group has a parent
        /// </summary>
        public int ParentID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string DetailedDescription { get; set; }

        public int Priority { get; set; }

        public bool Active { get; set; }

        public bool IsPublished { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Tags can be applied to menuitems and can have various meanings. 
    /// </summary>
    public class RMenuTag
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// If a tag is a "category", it generally means that this tag is generally something that can be displayed to the user
        /// An example would be "Spicy". 
        /// Tags that are not "categories" are items that have more of a logical nature to the system, such as tags are used to identify items
        /// that can belong to a specific combo plate.
        /// 
        /// </summary>
        public bool IsCategory { get; set; }

        /// <summary>
        /// Arbitrary string we can set on the category or use by the front end.
        /// </summary>
        public string MetaData { get; set; }
    }
}

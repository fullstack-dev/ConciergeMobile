using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ShoppingListItem
    {
        /// <summary>
        /// The unique ID of the item
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// You can override the list defaults here.
        /// </summary>
        public bool? BrandSubstitution { get; set; }

        /// <summary>
        /// You can override the list defaults here.
        /// </summary>
        public bool? Generic { get; set; }


        /// <summary>
        /// You can override the list defaults here.
        /// </summary>
        public bool? SizeSubstitution { get; set; }

        /// <summary>
        /// You can override the list defaults here.
        /// </summary>
        public bool? AllowSmallerSize { get; set; }

        /// <summary>
        /// You can override the list defaults here.
        /// </summary>
        public bool? AllowLargerSize { get; set; }

        public ShoppingProduct Product { get; set; }

        public int Quantity { get; set; }

        public string Note { get; set; }

    }
}

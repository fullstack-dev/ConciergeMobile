using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ShoppingList
    {
        /// <summary>
        /// The unique ID for this shopping list
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Flag that determines if the list should show up in the users list of Saved Shopping lists.
        /// </summary>
        public bool Browseable { get; set; }

        /// <summary>
        /// The name for this list. If <see cref="Browseable"/> it set, this filed must be non-empty.
        /// </summary>
        public string Name { get; set; }

        public IList<ShoppingListItem> Items { get; set; }

        public ShoppingList()
        {
            Items = new List<ShoppingListItem>();
        }

    }
}

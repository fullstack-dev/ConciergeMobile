using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledShoppingChargeItem
    {
        public int ID { get; set; }

        public decimal BaseChargeAmount { get; set; }

        public int? TaxRateID { get; set; }

        /// <summary>
        /// This ID of the shopping list item to which this charge applies.
        /// </summary>
        public int ShoppingListItemID { get; set; }
    }
}

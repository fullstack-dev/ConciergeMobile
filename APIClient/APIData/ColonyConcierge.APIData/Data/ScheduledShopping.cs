using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledShopping : ScheduledService
    {
        public bool Delivery { get; set; }

        public ShoppingType Type { get; set; }

        public enum ShoppingType
        {
            Grocery,
            Household,
            General
        }

        public bool IsWeekly { get; set; }

        /// <summary>
        /// If specified, is the ID of a store that the user has selected to use for the shopping service.
        /// </summary>
        public int? ShoppingStoreID { get; set; }

        public ShoppingPreference ShoppingPreference { get; set; }

        public ShoppingList ShoppingList { get; set; }

    }
}

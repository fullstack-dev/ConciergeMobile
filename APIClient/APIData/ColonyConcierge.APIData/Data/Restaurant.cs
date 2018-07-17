using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This is the root-level data for a restaurant.
    /// A restaurant consists of one or more locations.
    /// </summary>
    public class Restaurant
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool Active { get; set; }

        public bool IsPublished { get; set; }

        /// <summary>
        /// Relative price scale, range 1-5 with 5 being the most expensive.
        /// Note the restaurant locations can have price scale as well to account for relative expense.
        /// </summary>
        public int? PriceScale { get; set; }

        public string DetailedDescription { get; set; }

        public List<RestaurantCategory> Categories { get; set; }

    }
}

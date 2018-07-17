using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This class represents dynamic "on the fly" data related to search results when searching for locations.
    /// This data is computed on the fly by the search API and may be null for certain APIs when it is not relevant.
    /// </summary>
    public class RestaurantSearchResultData
    {
        /// <summary>
        /// The distance to the restaurant, in meters, from the search location.
        /// </summary>
        public decimal DistanceInMeters { get; set; }

        /// <summary>
        /// This is true if this restaurant location offers deliver to the search location.
        /// </summary>
        public bool IsDeliveryAvailable { get; set; }

        /// <summary>
        /// This is true, if this restaurant location is within the search radius from the search location
        /// </summary>
        public bool IsPickupAvailable { get; set; }

        public string RestaurantName { get; set; }

        public string RestaurantDisplayName { get; set; }

        public bool IsGroupedDeliveryAvailable { get; set; }

        public List<RestaurantCategory> ResaurantCategories { get; set; }

        public bool IsOpenForDelivery { get; set; }

        public bool IsOpenForPickup { get; set; }

        public TimeStamp NextDeliveryTime { get; set; }

        public TimeStamp NextDeliveryCutoff { get; set; }

        public TimeStamp NextPickupTime { get; set; }

        public TimeStamp NextPickupCutoff { get; set; }


    }
}

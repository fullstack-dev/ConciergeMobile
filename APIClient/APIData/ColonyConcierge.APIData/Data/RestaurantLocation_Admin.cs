using System;
using System.Collections.Generic;
using System.Linq;

namespace ColonyConcierge.APIData.Data
{
    public class RestaurantLocation_Admin
    {
        public int ID { get; set; }

        public int RestaurantID { get; set; }

        public bool Active { get; set; }

        public bool IsPublished { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public virtual string LogoUrl { get; set; }

        public string DetailedDescription { get; set; }

        public Address Address { get; set; }

        public string MainVoicePhone { get; set; }

        public string MainFaxPhone { get; set; }

        public int? PriceScale { get; set; }

        public decimal TaxRate { get; set; }

        //We're going to withhold these for now, it's an implementation detail we might not want to expose...
        //public int ServiceRadiusFeet { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public int Priority { get; set; }

        public decimal? DeliveryRadius { get; set; }


        public ServiceDaysOfWeek DaysOfWeekOpen { get; set; }

        /// <summary>
        /// The timezone of the restaurant location.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// How much time, in minutes, of lead time is needed to place a pickup order referencing a slot.
        /// </summary>
        public int PickupOrderLeadTimeTimeMinutes { get; set; }

        public int PickupOrderPaymentLeadTimeMinutes { get; set; }

        /// <summary>
        /// How much time, in minutes, of lead time is needed to place a delivery order referencing a slot.
        /// </summary>
        public int DeliveryOrderLeadTimeMinutes { get; set; }

        public int DeliveryOrderPaymentLeadTimeMinutes { get; set; }

        public int DeliveryDriverLeadTimeMinues { get; set; }

        public bool AllowExtendedDeliveryZone { get; set; }


        /// <summary>
        /// Currently a READ ONLY list of ID's of associated <see cref="Service"/> service definitions associated with this
        /// Location. 
        /// </summary>
        public List<int> ServiceIDs { get; set; }

        public int MaxOrdersPerDeliveryDriver { get; set; }

        public int PlanAheadDays { get; set; }

        /// <summary>
        /// This data is computed on the fly by the search API and may be null for certain APIs when it is not relevant.
        /// </summary>
        public RestaurantSearchResultData SearchResultData { get; set; }

        public int ScheduleGroup_ID { get; set; }

        public RestaurantLocation_Admin()
        {
            ServiceIDs = new List<int>();
        }
    }
}

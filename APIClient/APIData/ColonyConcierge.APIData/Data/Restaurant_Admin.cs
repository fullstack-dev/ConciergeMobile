using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ColonyConcierge.APIData.Data
{
    [DataContract]
    public class Restaurant_Admin
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool IsPublished { get; set; }

        /// <summary>
        /// Relative price scale, range 1-5 with 5 being the most expensive.
        /// Note the restaurant locations can have price scale as well to account for relative expense.
        /// </summary>
        [DataMember]
        public int? PriceScale { get; set; }

        [DataMember]
        public string DetailedDescription { get; set; }

        [DataMember]
        public List<RestaurantCategory> Categories { get; set; }
    }
}

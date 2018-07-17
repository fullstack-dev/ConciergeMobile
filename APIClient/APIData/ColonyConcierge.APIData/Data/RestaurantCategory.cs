using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ColonyConcierge.APIData.Data
{
    [DataContract]
    public class RestaurantCategory
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }
    }
}

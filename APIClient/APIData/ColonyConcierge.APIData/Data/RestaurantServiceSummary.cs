using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class RestaurantServiceSummary
    {
        public int RestaurantID { get; set; }

        public string RestaurantDisplayName { get; set; }

        public int LocationID { get; set; }

        public string LocationDisplayName { get; set; }

        public int DestinationID { get; set; }

        public string DestinationDisplayName { get; set; }

        public List<ScheduledRestaurantService> Services { get; set; }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class RestaurantLocationHoliday
    {

        public int ID { get; set; }

        public TimeStamp StartTime { get; set; }

        public TimeStamp EndTime { get; set; }


        public string Description { get; set; }

        public string SpecialNote { get; set; }
    }
}

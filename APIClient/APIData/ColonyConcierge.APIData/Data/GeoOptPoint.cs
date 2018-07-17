using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This is an "optimized" shape-related structure that is derived from submitted geo shapes. 
    /// Most client  will probably not need to ever use this type, it can be used in administrative and development tools.
    /// </summary>
    public class GeoOptPoint
    {
        public int ID { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

    }
}

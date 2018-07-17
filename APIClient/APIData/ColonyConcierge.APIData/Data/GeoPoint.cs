using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    [DebuggerDisplay("Lon = {Longitude}, Lat = {Latitude}")]
    public class GeoPoint
    {

        public int ID { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}

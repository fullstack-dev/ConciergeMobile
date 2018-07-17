using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents an address with additional information such as "Name" and Lat/Long
    /// </summary>
    public class ExtendedAddress
    {
        public int ID { get; set; }

        public Address BasicAddress { get; set; }

        public string Name { get; set; }


        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public bool IsPreferred { get; set; }
    }
}

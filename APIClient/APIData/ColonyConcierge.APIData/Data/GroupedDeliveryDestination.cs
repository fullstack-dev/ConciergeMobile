using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Specifies potential destination for an order delivery 
    /// </summary>
    public class GroupedDeliveryDestination
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public ExtendedAddress Address { get; set; }
    }
}

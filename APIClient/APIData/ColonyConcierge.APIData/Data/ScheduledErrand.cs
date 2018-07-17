using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledErrand : ScheduledService
    {
        public ErrandType Type { get; set; }

        public enum ErrandType
        {
            DryCleaning,
            CarDelivery,
            PetServices,
            Other
        }

        [Obsolete("DO NOT USE this , it is going away, use the 'SpecialInstructions' member on the server instead", true)]
        public string Description { get; set; }

        public string ErrandLocationName { get; set; }

        public Address ErrandLocation { get; set; }

        public bool Dropoff { get; set; }

        public bool Pickup { get; set; }
        /// <summary>
        /// <note type="note">This may change to a date....?</note>
        /// </summary>
        public TimeStamp DropoffTime { get; set; }

        public TimeStamp PickupTime { get; set; }
    }
}

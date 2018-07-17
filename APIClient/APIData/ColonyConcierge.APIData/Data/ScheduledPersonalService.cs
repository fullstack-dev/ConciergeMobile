using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledPersonalService : ScheduledService
    {
        public enum PersonalServiceType
        {
            Laundry,
            Waiting,
            HouseSitting,
            HandyMan,
            SnowBird
        }

        public PersonalServiceType Type { get; set; }

        /// <summary>
        /// Only applicable when Type == Laundry
        /// </summary>
        public string DetergentType { get; set; }

        /// <summary>
        /// Only Applicable when Type == Laundry
        /// </summary>
        public bool Delivery { get; set; }

        /// <summary>
        /// Only Applicable when Type == HandyMan
        /// </summary>
        public string WaitingFor { get; set; }


        /// <summary>
        /// Only Applicable when Type == HouseSitting
        /// </summary>
        public SimpleDate EndDate { get; set; }

        /// <summary>
        /// Only Applicable when Type == HouseSitting
        /// </summary>
        public bool WaterPlants { get; set; }

        /// <summary>
        /// Only Applicable when Type == HouseSitting
        /// </summary>
        public bool FeedPets { get; set; }

        /// <summary>
        /// Only Applicable when Type == HouseSitting
        /// </summary>
        public bool CheckMail { get; set; }

        /// <summary>
        /// Only Applicable when Type == HouseSitting
        /// </summary>
        public bool NewsPaper { get; set; }

        /// <summary>
        /// Only Applicable when Type == HouseSitting
        /// </summary>
        public bool TurnOnLights { get; set; }

        /// <summary>
        /// Only Applicable when Type == HouseSitting
        /// </summary>
        public bool OtherHouseSitting { get; set; }

    }
}

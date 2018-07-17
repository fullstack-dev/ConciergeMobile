using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class Service
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Title { get; set; }

        public ServiceTypes ServiceType { get; set; }

        public ServiceKindCodes ServiceKind { get; set; }

        public string DetailedDescription { get; set; }

        public List<ServiceFeePrice> Fees { get; set; }

        /// <summary>
        /// If this list contains a value for "0.00", then it should be interpreted that an "other" tip amount of arbitrary value is allowed.    
        /// </summary>
        public List<decimal> AllowedTipRates { get; set; }


        /// <summary>
        /// If not null, specifies a minimum tip rate.
        /// </summary>
        public decimal? MinimumTipRate { get; set; }


        /// <summary>
        /// If not null, specifies a minimum tip flat amount.
        /// </summary>
        public decimal? MinimumTipAmount { get; set; }

        /// <summary>
        /// If true, this service allows deliveries to "extended" service areas.
        /// </summary>
        public bool AllowExtendedDeliveryZone { get; set; }
    }
}

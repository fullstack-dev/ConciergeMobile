using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents the calculated service fees the are applicable to a service
    /// </summary>
    public class CalculatedServiceFee
    {
        /// <summary>
        /// Represents individual items
        /// </summary>
        public List<CalculatedServiceFeeLineItem> LineItems { get; set; }

        public CalculatedServiceFee()
        {
            LineItems = new List<CalculatedServiceFeeLineItem>();
        }

    }
}

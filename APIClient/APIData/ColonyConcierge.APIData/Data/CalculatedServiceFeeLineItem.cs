using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{

    /// <summary>
    /// Represents an individual item in a service fee calculation.
    /// </summary>
    public class CalculatedServiceFeeLineItem
    {
        public decimal Amount { get; set; }

        public string Description { get; set; }
    }
}

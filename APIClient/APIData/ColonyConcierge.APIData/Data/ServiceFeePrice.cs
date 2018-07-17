using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ServiceFeePrice
    {
        /// <summary>
        /// A descriptive name, meant to be used by the logic. Caution! This isn't necessarily unique!
        /// </summary>
        public string Name {get; set; }
        public decimal Amount { get; set; }

        public bool CanBeDiscounted { get; set; }
        /// <summary>
        /// This can also be treated like a "Display Name"
        /// </summary>
        public string Description { get; set; }
        public string InDepthDescription { get; set; }
    }
}

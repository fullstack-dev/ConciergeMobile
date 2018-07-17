using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// A tax rate that can be applied to goods or services. Defined as part of the service definition.
    /// </summary>
    public class TaxRate
    {
        public int ID { get; set; }

        /// <summary>
        /// A unique name for this tax rate (not intended for display)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A human-readable description of the tax
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The tax rate exressed as a fraction of a whole number. For example, a '7%' tax would be 0.07
        /// </summary>
        public decimal Rate { get; set; }
    }
}

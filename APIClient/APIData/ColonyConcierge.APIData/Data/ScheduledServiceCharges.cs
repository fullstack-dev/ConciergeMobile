using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Base class for adding charges to scheduled services.
    /// </summary>
    public class ScheduledServiceCharges : InheritedTypeBase
    {
        public int ID { get; set; }

        public decimal AdditionalFee { get; set; }
    }
}

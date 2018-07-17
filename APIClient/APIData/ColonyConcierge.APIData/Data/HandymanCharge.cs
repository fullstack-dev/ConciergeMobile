using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class representing charges to handyman related person services
    /// </summary>
    public class HandymanCharge
    {
        public int ID { get; set; }
        
        public string Description { get; set; }

        public decimal Charge { get; set; }

        public TimeStamp Time { get; set; }
    }
}

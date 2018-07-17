using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledSpecialRequest : ScheduledService
    {
        public string DetailedInstructions { get; set; }
    }
}

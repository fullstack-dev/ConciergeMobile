using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class JobHistoryEntry
    {
        public int ID { get; set; }

        public TimeStamp Time { get; set; }

        public string Note { get; set; }
    }
}

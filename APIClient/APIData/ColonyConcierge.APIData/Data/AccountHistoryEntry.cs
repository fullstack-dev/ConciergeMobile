using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class AccountHistoryEntry
    {
        public int ID { get; set; }

        public int AccountID { get; set; }

        public int ActiveUserID { get; set; }

        public int AccountUserID { get; set; }

        public TimeStamp Time { get; set; }

        public string Note { get; set; }
    }
}

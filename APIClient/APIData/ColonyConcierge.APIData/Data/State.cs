using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class State
    {
        public int ID { get; set; }

        public string City { get; set; }

        public string ServiceDisplayName { get; set; }

        public string ServiceDetailed { get; set; }

        public string StateFullName { get; set; }

        public string ServiceName { get; set; }

        public ServiceTypes ServiceType { get; set; }

        public ServiceKindCodes ServiceKind { get; set; }

        public string StateFull { get; set; }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class RMenu_Admin
    {
        public int ID { get; set; }

        public bool Active { get; set; }

        public bool IsPublished { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Disclaimer { get; set; }

        public int Priority { get; set; }
    }
}

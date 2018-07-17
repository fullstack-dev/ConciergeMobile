using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ShoppingStore
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Category { get; set; }

        public bool Active { get; set; }
    }
}

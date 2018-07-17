using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Data related to restaurant menus
    /// </summary>
    public class RMenu
    {
        public int ID { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Disclaimer { get; set; }


    }
}

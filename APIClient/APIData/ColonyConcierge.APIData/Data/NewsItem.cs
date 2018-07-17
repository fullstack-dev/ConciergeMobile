using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class that contains one piece of news for display on the website.
    /// </summary>
    public class NewsItem
    {

        public int ID { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public int Priority { get; set; }

        public SimpleDate Start { get; set; }

        public SimpleDate End { get; set; }

        public int? ServiceGroupID { get; set; }
    }
}

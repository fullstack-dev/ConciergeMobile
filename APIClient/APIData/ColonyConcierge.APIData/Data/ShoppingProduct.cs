using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ShoppingProduct
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string Size { get; set; }

        public string UPCCode { get; set; }

        /// <summary>
        /// NOT SUPPORTED AT THIS TIME, for possible future use
        /// </summary>
        public string EncodedScanImage { get; set; }

    }
}

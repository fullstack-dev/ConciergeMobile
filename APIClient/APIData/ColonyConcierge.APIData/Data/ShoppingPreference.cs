using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ShoppingPreference
    {
        public int ID { get; set; }

        public bool DefaultBrandSubstitution { get; set; }

        public bool DefaultGenericSubstitution { get; set; }

        public bool DefaultSizeSubstitution { get; set; }

        public bool DefaultCanSubstituteLarger { get; set; }

        public bool DefaultCanSubstituteSmaller { get; set; }

        public bool DefaultDelivery { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This is an "optimized" shape structure that is derived from submitted geo shapes. 
    /// Most client  will probably not need to ever use this type, it can be used in administrative and development tools.
    /// </summary>
    public class GeoConvexShape
    {
        public int ID { get; set; }

        public decimal NorthernBoundary { get; set; }

        public decimal SouthernBoundary { get; set; }

        public decimal EasternBoundary { get; set; }

        public decimal WesternBoundary { get; set; }

        /// <summary>
        /// Nonsensical unit that we use to sort the shapes when testing with the idea that larger shapes are more likely to contain the 
        /// point we are searching on.
        /// This is the area of the bounding box
        /// </summary>

        public decimal BoundingBoxArea { get; set; }

        /// <summary>
        /// Nonsensical unit that we use to sort the shapes when testing with the idea that larger shapes are more likely to contain the 
        /// point we are searching on.
        /// This is the area of the convex shape itself.
        /// </summary>
        public decimal ConvexArea { get; set; }


        public List<GeoOptPoint> Points { get; set; }
        


    }
}

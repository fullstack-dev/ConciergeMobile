using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class GeoShape
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public int Layer { get; set; }

        public List<GeoPoint> Points { get; set; }


        /// <summary>
        /// <note type="note">READ ONLY property, computed on the server when the geoshape was uploaded to the server. Clients cannot set this. </note>
        /// </summary>
        public decimal CenterLat { get; set; }

        /// <summary>
        /// <note type="note">READ ONLY property, computed on the server when the geoshape was uploaded to the server. Clients cannot set this. </note>
        /// </summary>
        public decimal CenterLong { get; set; }

        /// <summary>
        /// <note type="note">READ ONLY property, computed on the server when the geoshape was uploaded to the server. Clients cannot set this. </note>
        /// </summary>
        public decimal NorthernBoundary { get; set; }

        /// <summary>
        /// <note type="note">READ ONLY property, computed on the server when the geoshape was uploaded to the server. Clients cannot set this. </note>
        /// </summary>
        public decimal SouthernBoundary { get; set; }

        /// <summary>
        /// <note type="note">READ ONLY property, computed on the server when the geoshape was uploaded to the server. Clients cannot set this. </note>
        /// </summary>
        public decimal EasternBoundary { get; set; }

        /// <summary>
        /// <note type="note">READ ONLY property, computed on the server when the geoshape was uploaded to the server. Clients cannot set this. </note>
        /// </summary>
        public decimal WesternBoundary { get; set; }
    }
}

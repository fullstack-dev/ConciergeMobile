using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// An address object that can be used in many places in the API that have associated addresses.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Unique ID of this address entry
        /// </summary>
        public int ID
        { get; set; }

        /// <summary>
        /// First line of street address
        /// </summary>
        public string Line1
        { get; set; }

        /// <summary>
        /// Second line of street address
        /// </summary>
        public string Line2
        { get; set; }

        /// <summary>
        /// Third line of street address
        /// </summary>
        public string Line3
        { get; set; }

        /// <summary>
        /// Address city
        /// </summary>
        public string City
        { get; set; }

        /// <summary>
        /// Address Zip code.
        /// </summary>
        public string ZipCode
        { get; set; }

        /// <summary>
        /// Address state or province
        /// </summary>
        public string StateProv
        { get; set; }

        /// <summary>
        /// Address country. a 'null' value is assumed to be "United States"
        /// </summary>
        public string Country
        { get; set; }

        /// <summary>
        /// Returns this object represented in a single formatted string.
        /// Note that this is note really "official" and does not handle country-specific formats.
        /// It is US-centric in its fomratting.
        /// </summary>
        /// <returns></returns>
        public string ToCannonicalString(bool excludeLine1 = false)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Line1) && !excludeLine1)
            {
                sb.AppendLine(Line1);
            }

            if (!string.IsNullOrEmpty(Line2))
            {
                sb.AppendLine(Line2);
            }

            if (!string.IsNullOrEmpty(Line3))
            {
                sb.AppendLine(Line3);
            }

            if (!string.IsNullOrEmpty(City))
            {
                sb.Append(City + ", ");
            }

            if (!string.IsNullOrEmpty(StateProv))
            {
                sb.Append(StateProv + ", ");
            }

            if (!string.IsNullOrEmpty(ZipCode))
            {
                sb.Append(" " + ZipCode);
            }

            if (!string.IsNullOrEmpty(Country))
            {
                sb.AppendLine("");
                sb.Append(Country);
            }

            return sb.ToString();

        }
    }
}

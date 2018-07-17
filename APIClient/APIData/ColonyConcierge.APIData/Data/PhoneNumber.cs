using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class PhoneNumber
    {
        public int ID { get; set; }

        /// <summary>
        /// Limited to 32 characters.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// This can be any 16 characters the client wants it to be.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Useful for sorting, choosing 'best' number etc
        /// </summary>
        public int Priority { get; set; }
    }
}

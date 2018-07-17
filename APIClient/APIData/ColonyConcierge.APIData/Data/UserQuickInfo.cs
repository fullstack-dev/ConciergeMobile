using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Provides an abbreviated set of user information for quick display an lookup.
    /// </summary>
    public class UserQuickInfo
    {
        public int ID { get; set; }

        /// <summary>
        /// Is a combination of the user's first, middle and last names.
        /// </summary>
        public string DisplayName { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public bool IsWorker { get; set; }


    }
}

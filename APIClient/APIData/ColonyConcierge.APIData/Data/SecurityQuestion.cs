using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class SecurityQuestion
    {
        public int ID { get; set; }
        public string Question { get; set; }

        /// <summary>
        /// This will always be unpopulated when coming from the API server.
        /// Security answers are one-way encrypted on the server, there is no way to
        /// retrieve the original answer text.
        /// Plain text answers can be sent to the server when adding a new question
        /// </summary>
        public string Answer { get; set; }
    }
}

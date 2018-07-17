using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Represents messages consumable by users and administrators.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The unique message ID
        /// </summary>
        public int ID
        { get; set; }

        /// <summary>
        /// The username of the sender.
        /// </summary>
        public string Sender { get; set;}

        /// <summary>
        /// A list of usernames as recipients.
        /// </summary>
        public List<string> Recipients { get; set; }

        /// <summary>
        /// The subject line to display to users.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// The detailed body off the message.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Flag to mark a message as Read
        /// </summary>
        public bool Read { get; set; }

        /// <summary>
        /// Flag to mark a message as Important
        /// </summary>
        public bool Important { get; set; }

    }
}

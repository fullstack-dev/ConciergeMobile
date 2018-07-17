using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// A class to hold a note about an item.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// The unique ID for the note
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The creation/modification time for the note. This is set by the server.
        /// </summary>
        public TimeStamp NoteDate { get; set; }

        /// <summary>
        /// The content of the note. This is currently limited to 128 characters.
        /// </summary>
        public string Content { get; set; }
    }
}

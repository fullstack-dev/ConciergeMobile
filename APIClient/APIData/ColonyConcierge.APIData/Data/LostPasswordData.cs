using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Used to reset a password for a lost login password.
    /// </summary>
    public class LostPasswordData
    {
        /// <summary>
        /// The ID of the quesiton 
        /// </summary>
        public int QuestionID
        {
            get;
            set;
        }

        /// <summary>
        /// The plain-text answer supplied by users. Plain text answers are sent only from client to server, and are hashed and salted when stored.
        /// There is no way to retrieve existing answers from the server.
        /// </summary>
        public string Answer
        {
            get;
            set;
        }

        /// <summary>
        /// The plain text password to set a new password, for the appropriate APIS. Plain text passwords are sent only form the cleint to the server
        /// an are hashed and salted when stored. 
        /// There is no way to retrieve existing passwords from the server.
        /// </summary>
        public string NewPassword
        {
            get;
            set;
        }

    }
}

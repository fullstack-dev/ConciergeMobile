using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class User
    {
        /// <summary>
        /// The unique ID of this user
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// The unique ID of the string associate with this user.
        /// </summary>
        public int AccountID
        {
            get;
            set;
        }

        /// <summary>
        /// The login username
        /// </summary>
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// The user's real first name
        /// </summary>
        public string FirstName
        {
            get;
            set;
        }


        /// <summary>
        /// The user's real middle name
        /// </summary>
        public string MiddleName
        {
            get;
            set;
        }

        /// <summary>
        /// The user's real last name
        /// </summary>
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// The user's email address.
        /// <note>This must be unique within the system. No two users can have the same email address</note>
        /// </summary>
        public string EmailAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the user ID as a string
        /// </summary>
        /// <returns>a string value representing the ID</returns>
        public override string ToString()
        {
            return ID.ToString();
        }

        /// <summary>
        /// The user's Android device token.
        /// <note></note>
        /// </summary>
        public string AndroidToken
        {
            get;
            set;
        }

        /// <summary>
        /// The user's iOS device token.
        /// <note></note>
        /// </summary>
        public string iOSToken
        {
            get;
            set;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This is the main class for creating new users. 
    /// </summary>
    public class RegistrationEntry
    {
        /// <summary>
        /// The username for the new user, must be unique in the system.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password for the new user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The First name of the new user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The Middle name for the new user
        /// </summary>
        public string Middle { get; set; }

        /// <summary>
        /// The Last name for the new user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The email address for the new user. This must be unique to the system
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The list of phone numbers for the new user.
        /// </summary>
        public List<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// The service address for the new user. This is ignored if adding to an existing account
        /// </summary>
        public ExtendedAddress ServiceAddress { get; set; }

        /// <summary>
        /// The list of security questions for this user.
        /// </summary>
        public string[] SecurityQuestions { get; set; }

        /// <summary>
        /// The list of security question answers for this user.
        /// </summary>
        public string[] SecurityAnswers { get; set; }

        /// <summary>
        /// The subscription plan for the new user. This is ignored if adding to an existing account
        /// </summary>
        public int SubscriptionPlanID { get; set; }

        /// <summary>
        /// This is for adding this user to an existing account. You specify the username or email address of someone on the account.
        /// The username specified must have the "ApproveSubusers" permission, or this call will fail.
        /// Also, if this is specified, the new user will have a "pending" role until the user is approved.
        /// </summary>
        public string AddToAccountOf { get; set; }



    }
}

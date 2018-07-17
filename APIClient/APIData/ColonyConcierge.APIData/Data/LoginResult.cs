using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Result class for logins to the API server.
    /// </summary>
    public class LoginResult : Result
    {

        [Flags]
        public enum Flag
        {
            None,
            /// <summary>
            /// The login failed because the user did not validate their email address
            /// </summary>
            UserNotValidated        = 0x1,
            UsernameOrEmailInUse    = 0x2,
            UserIsLockedOut         = 0x4,
            TestAccountsDisabled    = 0x8, 
            UserNotAWorker          = 0x10,
            RoleRequirementsNotMet  = 0x20


        }

        /// <summary>
        /// Contains a login token that can be saved in a session and reused at a later time for additional API calls.
        /// </summary>
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// Flag indicating various properties of a login attempt
        /// </summary>
        public Flag Flags
        {
            get;
            set;
        }

        public static LoginResult Bad(string message)
        {
            return new LoginResult()
            {
                OK = false,
                Message = message
            };
        }

        public bool IsValidationFailure()
        {
            return (Flags & Flag.UserNotValidated) == Flag.UserNotValidated;
        }

        public bool IsUserNameOrEmailInUser()
        {
            return (Flags & Flag.UsernameOrEmailInUse) == Flag.UsernameOrEmailInUse;
        }

        public bool IsUserLockedOut()
        {
            return (Flags & Flag.UserIsLockedOut) == Flag.UserIsLockedOut;
        }

    }
}

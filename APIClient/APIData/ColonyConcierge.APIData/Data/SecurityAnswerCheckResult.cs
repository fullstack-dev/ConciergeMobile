using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class SecurityAnswerCheckResult : Result
    {
        /// <summary>
        /// This may be false, even if the correct answer was supplied, if thee user false account was disabled or locked out, 
        /// See <see cref="Flags"/> for more details
        /// </summary>
        public bool IsOkToLogin { get; set; }

        public LoginResult.Flag Flags { get; set; }


        public bool IsValidationFailure()
        {
            return (Flags & LoginResult.Flag.UserNotValidated) == LoginResult.Flag.UserNotValidated;
        }

        public bool IsUserNameOrEmailInUser()
        {
            return (Flags & LoginResult.Flag.UsernameOrEmailInUse) == LoginResult.Flag.UsernameOrEmailInUse;
        }

        public bool IsUserLockedOut()
        {
            return (Flags & LoginResult.Flag.UserIsLockedOut) == LoginResult.Flag.UserIsLockedOut;
        }

    }
}

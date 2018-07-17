using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ColonyConcierge.Mobile.Customer
{
    public class Validator
    {
        public Validator()
        {
        }

        static String emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
           @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        static String numberRegex = @"^\d$";
        static String floatRegex = @"^\d+[\,\.]{0,1}\d*$";
        static String phoneRegex = @"^[\+]{0,1}\d{10,20}$";

        /// <summary>
        /// Verify if the string is a valid email.
        /// </summary>
        /// <returns><c>true</c>, if valid email was valid, <c>false</c> otherwise.</returns>
        /// <param name="stringToValidate">String to validate.</param>
        static public bool IsStringAsEmail(String stringToValidate)
        {
            if (!Validator.IsStringValid(stringToValidate)) return false;
            return (Regex.IsMatch(stringToValidate, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
        }

        /// <summary>
        /// Verify if the string is a valid number.
        /// </summary>
        /// <returns><c>true</c>, if number, <c>false</c> otherwise.</returns>
        /// <param name="stringToValidate">String to validate.</param>
        static public bool IsStringAsNumber(String stringToValidate)
        {
            if (!Validator.IsStringValid(stringToValidate)) return false;
            return (Regex.IsMatch(stringToValidate, numberRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))); ;
        }

        /// <summary>
        /// Ises the string as float.
        /// </summary>
        /// <returns><c>true</c>, if string as float was ised, <c>false</c> otherwise.</returns>
        /// <param name="stringToValidate">String to validate.</param>
        static public bool IsStringAsFloat(String stringToValidate)
        {
            if (!Validator.IsStringValid(stringToValidate)) return false;
            return (Regex.IsMatch(stringToValidate, floatRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))); ;
        }

        /// <summary>
        /// Ises the string as phone.
        /// </summary>
        /// <returns><c>true</c>, if string as phone was ised, <c>false</c> otherwise.</returns>
        /// <param name="stringToValidate">String to validate.</param>
        static public bool IsStringAsPhone(String stringToValidate)
        {
            if (!Validator.IsStringValid(stringToValidate)) return false;
            return (Regex.IsMatch(stringToValidate, phoneRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))); ;
        }

        /// <summary>
        /// Verify if the string is valid (not null and not zero length)
        /// </summary>
        /// <returns><c>true</c>, if valid string was ised, <c>false</c> otherwise.</returns>
        /// <param name="stringToValidate">String to validate.</param>
        static public bool IsStringValid(String stringToValidate)
        {
            if (stringToValidate == null) return false;

            String stringWithoutSpaces = stringToValidate.Replace(" ", "");
            if (stringWithoutSpaces.Length == 0) return false;

            return true;
        }

        static public bool AreStringsValids(String[] strings)
        {
            foreach (var stringToValidate in strings)
            {
                if (!Validator.IsStringValid(stringToValidate))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

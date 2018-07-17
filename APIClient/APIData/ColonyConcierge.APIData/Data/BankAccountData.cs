using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Structure used to add a new payment method as a bank account
    /// </summary>
    public class BankAccountData : PaymentAccountData
    {
        /// <summary>
        /// Routing number
        /// </summary>
        public string RoutingNumber { get; set; }

        /// <summary>
        /// Account number
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// What type of account this represents. 'false', for checking, 'true' for savings.
        /// </summary>
        public bool IsSavings { get; set; }
    }
}

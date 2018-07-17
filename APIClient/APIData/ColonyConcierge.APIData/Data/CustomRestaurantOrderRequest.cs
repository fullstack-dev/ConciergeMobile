using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This is a class that can be used for custom orders placed form the front end.
    /// It is intended to be used by the business mobile app 
    /// </summary>
    public class CustomRestaurantOrderRequest 
    {

        public ScheduledRestaurantService WrappedServiceRequest { get; set; }

        public PaymentAccountData PaymentData { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public string CustomerEmail { get; set; }


    }
}

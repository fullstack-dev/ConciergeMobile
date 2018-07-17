using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class NotificationRecipientInformation
    {
        public int ID { get; set; }

        public string EmailAddress { get; set; }

        public string SMSNumber { get; set; }

        public string VoiceNumber { get; set; }

        public string FaxNumber { get; set; }
    }
}

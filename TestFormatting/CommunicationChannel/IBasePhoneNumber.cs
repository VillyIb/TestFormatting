using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatting.CommunicationChannel
{
    public interface IBasePhoneNumber
    {
        /// <summary>
        /// Phone/Cellular -
        /// Country code: 
        /// - prefix plus sign + 
        /// - followed by 1 to 7 digits optionally with space separator(s).
        /// Mandatory
        /// </summary>
        string CountryCode { get; set; }

        /// <summary>
        /// Phone/Cellular -
        /// Area Code:
        /// - 1 to 5 digits
        /// Optional.
        /// </summary>
        string AreaCode { get; set; }

        /// <summary>
        /// All types
        /// Phone/Cellular -
        /// Subscriber Number:
        /// - digits: 0..9
        /// - dialing control characters: A a B b C c D d P p T t W w * # ! @ $ ?.
        /// - formatting characters: space, period, dash.
        /// Mandatory
        /// </summary>
        string SubscriberNumber { get; set; }


        void ServiceEnable(CommunicationServiceType communicationServiceType);
    }
}

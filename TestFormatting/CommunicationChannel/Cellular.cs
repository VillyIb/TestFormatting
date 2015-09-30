using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatting.CommunicationChannel
{
    /// <summary>
    /// Cellular Phone Number extends from PhoneNumber
    /// </summary>
    public class Cellular : BasePhoneNumber<Cellular>, IBasePhoneNumber
    {
        public override void ServiceSupported(out List<CommunicationServiceType> communicationServiceTypeList)
        {
            base.ServiceSupported(out communicationServiceTypeList);
            communicationServiceTypeList.Add(CommunicationServiceType.Sms);
            communicationServiceTypeList.Add(CommunicationServiceType.Cellular);
        }


        public override string ToString(string format, IFormatProvider formatProvider)
        {
            // the formatProvider is NOT uset
            //
            // Proposed use: formatProvider is different depending on the target device used to format SMS/FaceTime/....
            // - IOS
            // - Android
            // - Windows-phone
            // - 

            var result = String.Empty;

            if (!String.IsNullOrEmpty(format))
            {
                if (format.Length == 1)
                {
                    var formatCode = format.Substring(0, 1);

                    switch (formatCode)
                    {
                        case "S":
                        case "s":
                            {
                                // SMS phone number on smartphone always with country code.
                                // Syntax for prefilling message:
                                // <a href="sms:{full-phone-number}&body={message here}>visible link</a>
                                result = String.Format("sms:{0}", GenericId);
                            }
                            break;

                        case "Y": // full
                        case "y": // audio only
                            {
                                // Skype phone number on smartphone.
                                result = String.Format("callto://{0}", GenericId);
                            }
                            break;

                        default:
                            {
                                result = base.ToString(format, formatProvider);
                            }
                            break;
                    }

                }
            }


            return result;
        }

        public static bool TryParse(string source, out Cellular value)
        {
            PhoneNumber t1;
            var result = TryParse(source, out t1);

            value = result ? new Cellular(t1) : null;
            return result;
        }


        public Cellular()
        {
            ServiceEnable(CommunicationServiceType.Sms);
            ServiceEnable(CommunicationServiceType.Cellular);
        }

        protected Cellular(BasePhoneNumber value) : this()
        {
            AreaCode = value.AreaCode;
            CountryCode = value.CountryCode;
            SubscriberNumber = value.SubscriberNumber;
        }
    }
}

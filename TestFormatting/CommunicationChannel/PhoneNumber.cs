using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TestFormatting.CommunicationChannel
{
    public class PhoneNumber : BaceCommunicationChannel
    {
        // <see cref="https://en.wikipedia.org/wiki/E.164"/>
        // <see cref="https://en.wikipedia.org/wiki/List_of_country_calling_codes"/>
        // <see cref=":https://en.wikipedia.org/wiki/Microsoft_telephone_number_format"/>

        /// <summary>
        /// Phone/Cellular -
        /// Country code: 
        /// - prefix plus sign + 
        /// - followed by 1 to 7 digits optionally with space separator(s).
        /// Mandatory
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Phone/Cellular -
        /// Area Code:
        /// - 1 to 5 digits
        /// Optional.
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// All types
        /// Phone/Cellular -
        /// Subscriber Number:
        /// - digits: 0..9
        /// - dialing control characters: A a B b C c D d P p T t W w * # ! @ $ ?.
        /// - formatting characters: space, period, dash.
        /// Mandatory
        /// </summary>
        public string SubscriberNumber { get; set; }

        /// <summary>
        /// Valid for Phone and Cellular
        /// </summary>
        public string Concatenated
        {
            get
            {
                return String.Format("{0}{1}{2}"
                    , CountryCode.Replace(" ", "").Replace("-", "")
                    , (AreaCode ?? "").Replace(" ", "").Replace("-", "")
                    , SubscriberNumber.Replace(" ", "").Replace("-", "")
                    );
            }
        }


        protected override void ServiceSet(CommunicationServiceType communicationServiceType)
        {
            if(CommunicationServiceType.Cellular == communicationServiceType
                || CommunicationServiceType.Sms == communicationServiceType
                || CommunicationServiceType.Phone  == communicationServiceType
                )
                base.ServiceSet(communicationServiceType);
        }


        // --- IFormattable

        /// <summary>
        /// Formatting characers: 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
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
                        case "O":
                        case "o":
                            {
                                var services = new StringBuilder();
                                foreach (var phoneServiceType in ServiceTypeList)
                                {
                                    services.AppendFormat("{0}{1:G}"
                                        , services.Length == 0 ? "" : ","
                                        , phoneServiceType
                                        );
                                }

                                // Round trip format
                                result = String.Format("{0};{1};{2};{3};Z"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    , services
                                    );
                            }
                            break;

                        case "G": // long
                            {
                                // General Phone Number.
                                result = String.IsNullOrEmpty(AreaCode) ?
                                    // no area code
                                    String.Format("{0}  {2}"  // note significant double space
                                    , CountryCode
                                    // ReSharper disable once FormatStringProblem
                                    , AreaCode
                                    , SubscriberNumber
                                    )
                                    :
                                    // with area code
                                String.Format("{0} ({1}) {2}"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "g": // short - without country code
                            {
                                // General Phone Number.
                                result = String.IsNullOrEmpty(AreaCode) ?
                                    // no area code
                                    String.Format("{2}"
                                    // ReSharper disable FormatStringProblem
                                    , CountryCode
                                    , AreaCode
                                    // ReSharper restore FormatStringProblem
                                    , SubscriberNumber
                                    )
                                    :
                                    // with area code
                                String.Format("({1}) {2}"
                                    // ReSharper disable once FormatStringProblem
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "X": // XML fragment
                        case "x": // XML fragment
                            {
                                result = base.ToString(format, formatProvider);
                            }
                            break;

                        case "T":
                        case "t":
                            {
                                // Dialing phone number on smartphone.
                                result = String.Format("tel:{0}", Concatenated);
                            }
                            break;

                        case "S":
                        case "s":
                            {
                                // SMS phone number on smartphone always with country code.
                                // Syntax for prefilling message:
                                // <a href="sms:{full-phone-number}&body={message here}>visible link</a>
                                result = String.Format("sms:{0}", Concatenated);
                            }
                            break;

                        case "F": // full FaceTime 
                            {
                                // FaceTime phone number on smartphone.
                                result = String.Format("facetime:{0}", Concatenated);
                            }
                            break;

                        case "f": // FaceTime audio only
                            {
                                // FaceTime phone number on smartphone.
                                result = String.Format("facetime-audio:{0}", Concatenated);
                            }
                            break;

                        case "Y": // full
                        case "y": // audio only
                            {
                                // Skype phone number on smartphone.
                                result = String.Format("callto://{0}", Concatenated);
                            }
                            break;
                    }

                }
            }


            return result;
        }

    }
}

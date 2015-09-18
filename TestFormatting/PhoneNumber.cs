using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TestFormatting
{
    public class PhoneNumber : IFormattable
    {
        // <see cref="https://en.wikipedia.org/wiki/E.164"/>
        // <see cref="https://en.wikipedia.org/wiki/List_of_country_calling_codes"/>
        // <see cref=":https://en.wikipedia.org/wiki/Microsoft_telephone_number_format"/>


        /// <summary>
        /// Country code: 
        /// - prefix plus sign + 
        /// - followed by 1 to 7 digits optionally with space separator(s).
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Area Code:
        /// - 1 to 5 digits
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// Subscriber Number:
        /// - digits: 0..9
        /// - dialing control characters: A a B b C c D d P p T t W w * # ! @ $ ?.
        /// - formatting characters: space, period, dash.
        /// </summary>
        public string SubscriberNumber { get; set; }


        // --- IFormattable

        /// <summary>
        /// Formatting characers: 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // the

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
                                // Round trip format
                                result = String.Format("{0};{1};{2};Z"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "G": // long
                            {
                                // General Phone Number.
                                result = String.IsNullOrEmpty(AreaCode) ?
                                    // no area code
                                    String.Format("{0} {2}"
                                    , CountryCode
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
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    )
                                    :
                                // with area code
                                String.Format("({1}) {2}"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "X": // XML fragment
                        case "x": // XML fragment
                            {
                                // General Phone Number.
                                result = String.Format("<PhoneNumber><CountryCode>{0}</CountryCode><AreaCode>{1}</AreaCode><SubscriberNumber>{2}</SubscriberNumber></PhoneNumber>"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "T":
                        case "t":
                            {
                                // Dialing phone number on smartphone.
                                result = String.Format("tel:{0}{1}{2}"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "S":
                        case "s":
                            {
                                // SMS phone number on smartphone always with country code.
                                result = String.Format("sms:{0}{1}{2}"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "F": // full FaceTime 
                        case "f": // FaceTime audio only
                            {
                                // FaceTime phone number on smartphone.
                                result = String.Format("xxx:{0}{1}{2}"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        case "Y": // full
                        case "y": // audio only
                            {
                                // Skype phone number on smartphone.
                                result = String.Format("sms:{0}{1}{2}"
                                    , CountryCode
                                    , AreaCode
                                    , SubscriberNumber
                                    );
                            }
                            break;

                        default:
                            break;
                    }

                }
            }


            return result;
        }

        public bool TryParse(String source, out PhoneNumber result)
        {
            if (string.IsNullOrEmpty(source))
            {
                result = null;
                return false;
            }

            // Parse from O
            {
                // Expected "+CountryCode;AreaCode;SubscriberNumber;Z". 
                var arr = source.Split(new[] { ';' }, StringSplitOptions.None);
                if (arr.Length == 4)
                {
                    if ("Z".Equals(arr[3], StringComparison.OrdinalIgnoreCase))
                    {
                        result = new PhoneNumber { CountryCode = arr[0], AreaCode = arr[1], SubscriberNumber = arr[2] };
                        return true;
                    }
                }
            }

            // Parse from X
            {
                try
                {
                    var xml = new XmlDocument();
                    xml.InnerText = String.Format("<?xml version=\"1.0\" encoding=\"utf - 8\"?><root>{0}</root>", source);

                    var countryNode = xml.SelectSingleNode("CountryCode");
                    var areaNode = xml.SelectSingleNode("AreaCode");
                    var subscriberNode = xml.SelectSingleNode("SubscriberNumber");

                    if (countryNode != null && areaNode != null && subscriberNode != null)
                    {
                        result = new PhoneNumber { CountryCode = countryNode.InnerText, AreaCode = areaNode.InnerText, SubscriberNumber = subscriberNode.InnerText };
                        return true;
                    }
                }
                catch { }
            }

            // Parse from G            
            {
                do // do once with break options.
                {
                    // expected "+CountryCode [(AreaCode)] SubscriberNumber".  AreaCode is optional
                    //           a             b        c de           

                    var areaCode = String.Empty;

                    var pa = source.IndexOf("+");
                    if (pa == -1) { break; }

                    var pb = source.IndexOf("(");
                    if (pb != -1)
                    {
                        var pc = source.IndexOf(")", pb + 1);
                        if (pc == -1) { break; }

                        areaCode = source.Substring(0, 0);
                    }


                } while (false);

            }

            result = null;
            return false;
        }
    }
}

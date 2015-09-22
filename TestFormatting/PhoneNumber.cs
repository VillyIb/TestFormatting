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
        /// Mandatory
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Area Code:
        /// - 1 to 5 digits
        /// Optional.
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// Subscriber Number:
        /// - digits: 0..9
        /// - dialing control characters: A a B b C c D d P p T t W w * # ! @ $ ?.
        /// - formatting characters: space, period, dash.
        /// Mandatory
        /// </summary>
        public string SubscriberNumber { get; set; }

        public string Concatenated
        {
            get
            {
                return String.Format("{0}{1}{2}"
                    , CountryCode.Replace(" ","").Replace("-","")
                    , (AreaCode ?? "").Replace(" ", "").Replace("-", "")
                    , SubscriberNumber.Replace(" ", "").Replace("-", "")
                    );
            }
        }


        // --- IFormattable

        /// <summary>
        /// Formatting characers: 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
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
                                    String.Format("{0}  {2}"  // note significant double space
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
                                result = String.Format("facetime:{0}",Concatenated);
                            }
                            break;

                        case "f": // FaceTime audio only
                            {
                                // FaceTime phone number on smartphone.
                                result = String.Format("facetime-audio:{0}",Concatenated);
                            }
                            break;

                        case "Y": // full
                        case "y": // audio only
                            {
                                // Skype phone number on smartphone.
                                result = String.Format("callto://{0}",Concatenated);
                            }
                            break;

                        default:
                            break;
                    }

                }
            }


            return result;
        }

        public static bool TryParse(String source, out PhoneNumber result)
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
                    xml.InnerXml = String.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><root>{0}</root>", source);

                    var countryNode = xml.SelectSingleNode("//CountryCode");
                    var areaNode = xml.SelectSingleNode("//AreaCode");
                    var subscriberNode = xml.SelectSingleNode("//SubscriberNumber");

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
                // expected "+CountryCode [(AreaCode)] SubscriberNumber".  AreaCode is optional
                // expected "+CountryCode  SubscriberNumber".  AreaCode is optional Notice double space between CC and SN.

                var charArray = source.ToCharArray();

                var areaStart = source.IndexOf(" (", StringComparison.OrdinalIgnoreCase);

                var hasAreaCode = areaStart > -1;

                if (hasAreaCode)
                {
                    var countryCode = source.Substring(0, areaStart).Trim();

                    var areaEnd = source.IndexOf(") ", StringComparison.CurrentCultureIgnoreCase);

                    if (areaEnd == -1)
                    {
                        result = null;
                        return false;
                    }

                    var areaCode = source.Substring(areaStart + 2, areaEnd - areaStart - 2).Trim();

                    var subscriberNumber = source.Substring(areaEnd + 2).Trim();

                    result = new PhoneNumber
                    {
                        CountryCode = countryCode,
                        AreaCode = areaCode,
                        SubscriberNumber = subscriberNumber
                    };

                    return true;
                }
                else
                {
                    var ccEnd = source.IndexOf("  ", StringComparison.OrdinalIgnoreCase);
                    if (ccEnd == -1)
                    {
                        result = null;
                        return false;
                    }

                    var countryCode = source.Substring(0, ccEnd).Trim();

                    var subscriberNumber = source.Substring(ccEnd + 1).Trim();

                    result = new PhoneNumber
                    {
                        CountryCode = countryCode,
                        AreaCode = string.Empty,
                        SubscriberNumber = subscriberNumber
                    };

                    return true;
                }

            }
        }
    }
}

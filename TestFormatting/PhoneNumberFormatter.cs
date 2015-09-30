using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFormatting
{
    public class PhoneNumberFormatter : IFormatProvider, ICustomFormatter
    {

        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            else if (arg != null)
                return arg.ToString();
            else
                return String.Empty;
        }


        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Handle format string.
            int baseNumber;

            // Handle null or empty format string, string with precision specifier.
            string thisFmt = String.Empty;

            // Extract first character of format string (precision specifiers
            // are not supported).
            if (!String.IsNullOrEmpty(format))
            {
                thisFmt = format.Length > 1 ? format.Substring(0, 1) : format;
            }


            if (arg is CommunicationChanneXl)
            {
                //bytes = ((BigInteger)arg).ToByteArray();
            }
            else
            {
                try
                {
                    return HandleOtherFormats(format, arg);
                }
                catch (FormatException e)
                {
                    throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
                }
            }

            switch (format)
            {
                // PhoneNumber formatting.
                case "P": // long mode
                    baseNumber = 2;
                    break;
                case "p": // short mode
                    baseNumber = 8;
                    break;
                case "S": // SMS:
                case "s":
                    baseNumber = 16;
                    break;
                case "T": // Tel: 
                case "t":
                    baseNumber = 16;
                    break;

                    // -- 

                case "C": // Country Code C|c|cc|ccc|cccc|ccccc  filled from right
                    baseNumber = 16;
                    break;

                case "A": // Area Code  A|a{a}0..5 filled from right
                    baseNumber = 16;
                    break;

                case "N": // Subscriber Number  
                    baseNumber = 16;
                    break;

                // Handle unsupported format strings.
                default:
                    try
                    {
                        return HandleOtherFormats(format, arg);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
                    }
            }

            // Return a formatted string.
            string numericString = String.Empty;
            //for (int ctr = bytes.GetUpperBound(0); ctr >= bytes.GetLowerBound(0); ctr--)
            //{
            //    string byteString = Convert.ToString(bytes[ctr], baseNumber);
            //    if (baseNumber == 2)
            //        byteString = new String('0', 8 - byteString.Length) + byteString;
            //    else if (baseNumber == 8)
            //        byteString = new String('0', 4 - byteString.Length) + byteString;
            //    // Base is 16.
            //    else
            //        byteString = new String('0', 2 - byteString.Length) + byteString;

            //    numericString += byteString + " ";
            //}
            return numericString.Trim();
        }

        public object GetFormat(Type formatType)
        {
            // Determine whether custom formatting object is requested.
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }
    }
}

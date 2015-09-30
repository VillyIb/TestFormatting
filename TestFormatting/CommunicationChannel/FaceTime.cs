using System;

namespace TestFormatting.CommunicationChannel
{
    public class FaceTime : BaseCommunicationChannel
    {
        public String FaceTimeId { get; set; }


        public override string GenericId { get { return FaceTimeId; } }


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
                        case "F": // full FaceTime 
                            {
                                // FaceTime phone number on smartphone.
                                result = String.Format("facetime:{0}", GenericId);
                            }
                            break;

                        case "f": // FaceTime audio only
                            {
                                // FaceTime phone number on smartphone.
                                result = String.Format("facetime-audio:{0}", GenericId);
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


        public static bool TryParse(string source, out FaceTime value)
        {
            value = new FaceTime { FaceTimeId = source };
            return true;
        }


        public FaceTime()
        {
            ServiceEnable(CommunicationServiceType.FaceTime);
            ServiceEnable(CommunicationServiceType.FaceTimeAudio);
        }

    }
}

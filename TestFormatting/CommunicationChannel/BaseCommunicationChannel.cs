using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TestFormatting.CommunicationChannel
{
    public abstract class BaseCommunicationChannel
    {
        private List<CommunicationServiceType> zServiceTypeList;

        /// <summary>
        /// Holds the list of Enabled Services.
        /// </summary>
        //[XmlElement("ServiceList")]
        [XmlArray("ServiceList")]
        [XmlArrayItem("Service")]
        protected List<CommunicationServiceType> ServiceTypeList
        {
            get { return zServiceTypeList ?? (zServiceTypeList = new List<CommunicationServiceType>()); }
        }


        /// <summary>
        /// Returns true if the specified ServiceType is registered on the PhoneNumber.
        /// </summary>
        /// <param name="communicationServiceType"></param>
        /// <returns></returns>
        public virtual bool ServiceIsEnabled(CommunicationServiceType communicationServiceType)
        {
            return ServiceTypeList.Any(t => t.Equals(communicationServiceType));
        }


        /// <summary>
        /// Returns list of supported services.
        /// Override this method to provide specific Services.
        /// </summary>
        /// <param name="communicationServiceTypeList"></param>
        public virtual void ServiceSupported(out List<CommunicationServiceType> communicationServiceTypeList)
        {
            communicationServiceTypeList = new List<CommunicationServiceType>();
        }


        /// <summary>
        /// Enables service.
        /// </summary>
        /// <param name="communicationServiceType"></param>
        public void ServiceEnable(CommunicationServiceType communicationServiceType)
        {
            List<CommunicationServiceType> t1;
            ServiceSupported(out t1);

            if (t1.Any(t => t.Equals(communicationServiceType)))
            {
                if (!(ServiceIsEnabled(communicationServiceType)))
                {
                    ServiceTypeList.Add(communicationServiceType);
                }
            }
        }


        /// <summary>
        /// Disables service.
        /// </summary>
        /// <param name="communicationServiceType"></param>
        protected virtual void ServiceDisable(CommunicationServiceType communicationServiceType)
        {
            if (!(ServiceIsEnabled(communicationServiceType)))
            {
                ServiceTypeList.Remove(communicationServiceType);
            }
        }


        /// <summary>
        /// Generic representation of class Id.
        /// </summary>
        public abstract string GenericId { get; }

        public virtual string ToString(string format = "G", IFormatProvider formatProvider = null)
        {
            var result = String.Empty;

            if (String.IsNullOrEmpty(format)) return result;

            if (format.Length != 1) return result;

            var formatCode = format.Substring(0, 1);

            switch (formatCode)
            {
                case "G":
                case "g":
                    {
                        result = GenericId;
                    }
                    break;

                case "X": // XML fragment
                case "x": // XML fragment
                default:
                    {
                        var settings = new XmlWriterSettings
                        {
                            Encoding = new UnicodeEncoding(false, false),
                            Indent = true,
                            OmitXmlDeclaration = true,
                        };

                        var serializer = new XmlSerializer(GetType());

                        using (var stringWriter = new StringWriter())
                        {
                            using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                            {
                                serializer.Serialize(xmlWriter, this);
                                result = stringWriter.ToString();
                            }
                        }
                    }
                    break;

            }

            return result;
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TestFormatting.CommunicationChannel
{
    public class BaceCommunicationChannel : IFormattable
    {
        private List<CommunicationServiceType> zServiceTypeList;

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
        public virtual bool ServiceEnabled(CommunicationServiceType communicationServiceType)
        {
            return ServiceTypeList.Any(t => t.Equals(communicationServiceType));
        }


        protected virtual void ServiceSet(CommunicationServiceType communicationServiceType)
        {
            if (!(ServiceEnabled(communicationServiceType)))
            {
                ServiceTypeList.Add(communicationServiceType);
            }
        }


        protected virtual void ServiceClear(CommunicationServiceType communicationServiceType)
        {
            if (!(ServiceEnabled(communicationServiceType)))
            {
                ServiceTypeList.Remove(communicationServiceType);
            }
        }


        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            var result = String.Empty;

            if (String.IsNullOrEmpty(format)) return result;

            if (format.Length != 1) return result;

            var formatCode = format.Substring(0, 1);

            switch (formatCode)
            {
                case "X": // XML fragment
                case "x": // XML fragment
                    {
                        var settings = new XmlWriterSettings
                        {
                            Encoding = new UnicodeEncoding(false, false),
                            Indent = true,
                            OmitXmlDeclaration = true,
                        };

                        var serializer = new XmlSerializer(typeof(CommunicationChanneXl));

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

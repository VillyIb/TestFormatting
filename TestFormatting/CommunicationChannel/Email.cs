using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatting.CommunicationChannel
{
    public sealed class Email : BaceCommunicationChannel
    {
        public String EmailX { get; set; }

        protected override void ServiceSet(CommunicationServiceType communicationServiceType)
        {
            if (CommunicationServiceType.Email == communicationServiceType)
            {
                base.ServiceSet(communicationServiceType);
            }
        }

        public override bool ServiceEnabled(CommunicationServiceType communicationServiceType)
        {
            return CommunicationServiceType.Email == communicationServiceType;
        }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return base.ToString(format, formatProvider);
        }

        public Email()
        {
              ServiceSet(CommunicationServiceType.Email);
        }
    }
}

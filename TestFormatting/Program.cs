using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TestFormatting
{
    public class ship
    {
        public decimal Ship_Customervalue;
        public string Ship_Valuta = "abc";
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ship = new ship { Ship_Customervalue = new decimal(123456.78) };

            var Terms = "DDP";



            XElement duty = new XElement("Dutiable",
                new XElement("DeclaredValue", string.Format("{0:00}", ship.Ship_Customervalue).Replace(",", ".")),
                new XElement("DeclaredCurrency", ship.Ship_Valuta.Replace(",", ".")),
                (new XElement("TermsOfTrade", Terms))
            );

            XElement duty2 = new XElement("Dutiable",
                new XElement("DeclaredValue", ship.Ship_Customervalue.ToString("0.00", CultureInfo.InvariantCulture)),
                new XElement("DeclaredCurrency", ship.Ship_Valuta.Replace(",", "")),
                (new XElement("TermsOfTrade", Terms))
            );




            XElement duty3 = new XElement("Dutiable",

                new XElement("DeclaredValue", string.Format("{0:N2}", ship.Ship_Customervalue.ToString().Replace(".", "")).Replace(",", ".")),
                    new XElement("DeclaredCurrency", ship.Ship_Valuta.Replace(",", ".")),
                    (new XElement("TermsOfTrade", Terms))
                    );





            var msg1 = duty.ToString();
            var msg2 = duty2.ToString();
            var msg3 = duty3.ToString();
        }
    }
}

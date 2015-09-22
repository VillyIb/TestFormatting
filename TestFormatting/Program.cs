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
            PhoneNumber t1;
            var ph = PhoneNumber.TryParse("+45 7 (123) 21 17 45 05", out t1) ? t1 : new PhoneNumber();

            var ptG = ph.ToString("G", null);
            var ptg = ph.ToString("g", null);

            var ptX = ph.ToString("X", null);
            var ptx = ph.ToString("x", null);

            var ptO = ph.ToString("O", null);
            var pto = ph.ToString("o", null);

            var ptS = ph.ToString("S", null);
            var pts = ph.ToString("s", null);

            var ptT = ph.ToString("T", null);
            var ptt = ph.ToString("t", null);

            var ptY = ph.ToString("Y", null);
            var pty = ph.ToString("y", null);

            var ptF = ph.ToString("F", null);
            var ptf = ph.ToString("f", null);

            var phG = PhoneNumber.TryParse(ptG, out t1) ? t1 : null;
            var phx = PhoneNumber.TryParse(ptx, out t1) ? t1 : null;
            var pho = PhoneNumber.TryParse(pto, out t1) ? t1 : null;
            var phg = PhoneNumber.TryParse(ptg, out t1) ? t1 : null;
            var pht = PhoneNumber.TryParse(ptt, out t1) ? t1 : null;
            var phy = PhoneNumber.TryParse(pty, out t1) ? t1 : null;

            var phxx = phG;
            phxx = phx;
            phxx = pho;
            phxx = phg;
            phxx = pht;
            phxx = phy;

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

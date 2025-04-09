using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorWalutSroda.Klasy
{
    public class PozycjaTabA
    {
        public string NazwaWaluty { get; set; }

        public string KodWaluty { get; set; }

        public string Przelicznik { get; set; }

        public string KursSredni { get; set; }

        public decimal Kurs
        {
            get
            {
                decimal kurs;
                string separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                KursSredni = KursSredni.Replace(",", separator);
                bool wynik = decimal.TryParse(KursSredni, out kurs);
                return kurs;
            }
        }
    }
}

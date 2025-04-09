using KalkulatorWalutSroda.Klasy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace KalkulatorWalutSroda
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<PozycjaTabA> kursyWalut = new List<PozycjaTabA>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        void Przelicz()
        {
            if (WalutaWejscieLb.SelectedIndex == -1 || WalutaWyjscieLb.SelectedIndex == -1 || KwotaTbx.Text == "") return;

            var zWaluty = WalutaWejscieLb.SelectedItem as PozycjaTabA;
            var naWalute = WalutaWyjscieLb.SelectedItem as PozycjaTabA;

            decimal kwota;
            if( decimal.TryParse(KwotaTbx.Text, out kwota) )
            {
                BladTb.Visibility = Visibility.Collapsed;

                decimal naPLN = zWaluty.Kurs * kwota;
                decimal wynik = naPLN / naWalute.Kurs;

                PrzeliczonaTb.Text = string.Format($"{wynik:f2} {naWalute.KodWaluty}");
            }
            else
            {
                BladTb.Visibility = Visibility.Visible;
                KwotaTbx.Text = "";
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var klient = new HttpClient();
            try
            {
                var dane = await klient.GetStringAsync(new Uri(API_NBP.daneNBP));

                var daneXML = XDocument.Parse(dane);

                foreach (var pozycja in daneXML.Descendants("pozycja"))
                {
                    PozycjaTabA nowaPozycja = new PozycjaTabA()
                    {
                        NazwaWaluty = pozycja.Element("nazwa_waluty").Value,
                        KodWaluty = pozycja.Element("kod_waluty").Value,
                        Przelicznik = pozycja.Element("przelicznik").Value,
                        KursSredni = pozycja.Element("kurs_sredni").Value
                    };

                    kursyWalut.Add(nowaPozycja);
                }

                kursyWalut.Insert(0, new PozycjaTabA() { KodWaluty = "PLN", NazwaWaluty = "Złoty polski", Przelicznik = "1", KursSredni = "1,0000" });

                WalutaWejscieLb.ItemsSource = kursyWalut;
                WalutaWyjscieLb.ItemsSource = kursyWalut;
            }
            catch
            {
                var message = new ContentDialog()
                {
                    Title = "Błąd",
                    Content = "Nie udało się pobrać danych NBP\nSpróbuj ponownie później",
                    CloseButtonText = "OK"
                };

                await message.ShowAsync();

                Application.Current.Exit();
            }
        }

        private void KwotaTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            Przelicz();
        }

        private void Waluta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Przelicz();
        }
    }
}

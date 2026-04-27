using System;
using System.Windows;
using System.Windows.Media;
using LibrarieModele;
using NivelStocareDate; 

namespace FarmacieUI_WPF
{
    public partial class MainWindow : Window
    {
        private IStocareData adminMedicamente;

        // 1. Definim constantele pentru limitele de validare (Cerința din Tema Acasă)
        private const int LUNGIME_MAXIMA_DENUMIRE = 30;
        private const double PRET_MINIM = 0.1;
        private const int STOC_MINIM = 0;

        // 2. Enumerare pentru codurile de eroare
        private enum CodEroare
        {
            Valid = 0,
            DenumireInvalida = 1,
            PretInvalid = 2,
            StocInvalid = 3
        }

        public MainWindow()
        {
            InitializeComponent();

            // Inițializăm salvarea în fișier 
            // Dacă ai o clasă StocareFactory ca la studenți, o poți folosi. Altfel, instanțiem direct:
            adminMedicamente = new AdministrareMedicamenteFisierText("Medicamente.txt");
        }

        // Metoda de validare cu returnare de cod de eroare
        private CodEroare ValideazaDateMedicament(string denumire, string pretStr, string stocStr)
        {
            if (string.IsNullOrWhiteSpace(denumire) || denumire.Length > LUNGIME_MAXIMA_DENUMIRE)
                return CodEroare.DenumireInvalida;

            if (!double.TryParse(pretStr, out double pret) || pret < PRET_MINIM)
                return CodEroare.PretInvalid;

            if (!int.TryParse(stocStr, out int stoc) || stoc < STOC_MINIM)
                return CodEroare.StocInvalid;

            return CodEroare.Valid;
        }

        // Evenimentul pentru butonul Adaugă
        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            AscundeErori();
            lblMesajStatus.Content = "";

            string denumire = txtDenumire.Text.Trim();
            string pretStr = txtPret.Text.Trim();
            string stocStr = txtStoc.Text.Trim();

            // Apelăm metoda de validare
            CodEroare rezultatValidare = ValideazaDateMedicament(denumire, pretStr, stocStr);

            if (rezultatValidare != CodEroare.Valid)
            {
                // Schimbăm culorile dacă sunt erori
                MarcheazaEroare(rezultatValidare);
                return; // Oprim execuția, nu salvăm date greșite
            }

            // Transformăm string-urile în numere (știm că e sigur, au trecut de validare)
            double pret = Convert.ToDouble(pretStr);
            int stoc = Convert.ToInt32(stocStr);

            // Creăm obiectul (setăm Forma și Conditiile la niste valori default pentru acest formular simplu)
            Medicament medicamentNou = new Medicament(0, denumire, pret, stoc, FormaPrezentare.Comprimate, ConditiiPastrare.TemperaturaCamerei);

            // Salvăm în fișier
            adminMedicamente.AdaugaMedicament(medicamentNou);

            lblMesajStatus.Foreground = Brushes.Green;
            lblMesajStatus.Content = "Medicament adăugat cu succes!";

            CurataCampuri();
        }

        // Evenimentul pentru butonul Reset
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            CurataCampuri();
            AscundeErori();
            lblMesajStatus.Content = "";
        }

        // Metode ajutătoare pentru interfață
        private void CurataCampuri()
        {
            txtDenumire.Clear();
            txtPret.Clear();
            txtStoc.Clear();
        }

        private void MarcheazaEroare(CodEroare cod)
        {
            switch (cod)
            {
                case CodEroare.DenumireInvalida:
                    txtDenumire.Background = Brushes.LightPink;
                    tbErrDenumire.Text = $"Numele este obligatoriu și max {LUNGIME_MAXIMA_DENUMIRE} caractere!";
                    tbErrDenumire.Visibility = Visibility.Visible;
                    break;
                case CodEroare.PretInvalid:
                    txtPret.Background = Brushes.LightPink;
                    tbErrPret.Text = "Prețul trebuie să fie un număr valid, mai mare ca 0!";
                    tbErrPret.Visibility = Visibility.Visible;
                    break;
                case CodEroare.StocInvalid:
                    txtStoc.Background = Brushes.LightPink;
                    tbErrStoc.Text = "Stocul trebuie să fie un număr întreg pozitiv!";
                    tbErrStoc.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void AscundeErori()
        {
            txtDenumire.Background = Brushes.White;
            txtPret.Background = Brushes.White;
            txtStoc.Background = Brushes.White;

            tbErrDenumire.Visibility = Visibility.Collapsed;
            tbErrPret.Visibility = Visibility.Collapsed;
            tbErrStoc.Visibility = Visibility.Collapsed;
        }
    }
}
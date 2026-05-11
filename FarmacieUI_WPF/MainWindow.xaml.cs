using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibrarieModele;
using NivelStocareDate;

namespace NivelUIWPF
{
    // Am adaugat INotifyPropertyChanged pentru a folosi Data Binding pe entitatea Farmacie
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Administrare Medicamente (existent)
        private IStocareData adminMedicamente;

        // Administrare Farmacii 
        private IStocareFarmacii adminFarmacii;
        private Farmacie farmacieCurenta;

        // Proprietatea legata direct la XAML prin Binding
        public Farmacie FarmacieCurenta
        {
            get => farmacieCurenta;
            set
            {
                farmacieCurenta = value;
                OnPropertyChanged(); // Anuntam interfata cand se schimba farmacia
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();

            // Setam contextul de date pentru Binding
            DataContext = this;

            // Initializare Medicamente
            adminMedicamente = new AdministrareMedicamenteFisierText("Medicamente.txt");
            //AfiseazaInTabel(adminMedicamente.GetStoc()); Dezactivat ca sa nu incarce memoria in caz de fisiere mari
            lstCategorieAdaugare.ItemsSource = Medicament.CategoriiDisponibile;
            lstCategorieAdaugare.SelectedIndex = 0;
            dtpDataExpirare.SelectedDate = DateTime.Today.AddYears(1);

            // Initializare Farmacii 
            adminFarmacii = new AdministrareFarmaciiFisierText("Farmacii.txt");
            FarmacieCurenta = new Farmacie(); // Initializam o farmacie goala pentru a nu avea erori in XAML
            AfiseazaFarmacii();
        }


        // MENIU VERTICAL NAVIGARE

        private void btnMeniuAdauga_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouAdaugare.Visibility = Visibility.Visible;
            dgMedicamente.Visibility = Visibility.Visible; // Arată tabelul de medicamente
        }

        private void btnMeniuModifica_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouModifica.Visibility = Visibility.Visible;
            dgMedicamente.Visibility = Visibility.Visible; // Arată tabelul de medicamente

            cmbMedicamenteModificare.ItemsSource = null;
            cmbMedicamenteModificare.ItemsSource = adminMedicamente.GetStoc();
            txtModificaDenumire.Clear(); txtModificaPret.Clear(); txtModificaStoc.Clear();
            dtpModificaDataExpirare.SelectedDate = null;
        }

        private void btnMeniuCauta_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouCautare.Visibility = Visibility.Visible;
            dgMedicamente.Visibility = Visibility.Visible; // Arată tabelul de medicamente
        }

        private void btnMeniuFarmacii_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouFarmacii.Visibility = Visibility.Visible;

            // Ascundem tabelul de medicamente ca să facem loc panoului de farmacii
            dgMedicamente.Visibility = Visibility.Collapsed;

            FarmacieCurenta = new Farmacie(); // Formular gol cand deschidem panoul
        }

        private void AscundeToatePanourile()
        {
            panouAdaugare.Visibility = Visibility.Collapsed;
            panouModifica.Visibility = Visibility.Collapsed;
            panouCautare.Visibility = Visibility.Collapsed;
            panouFarmacii.Visibility = Visibility.Collapsed;
            lblStatus.Content = "";
        }


        // Logica pentru gestionarea farmaciilor - a doua entitate cu binding

        private void AfiseazaFarmacii()
        {
            dgFarmacii.ItemsSource = null;
            dgFarmacii.ItemsSource = adminFarmacii.GetFarmacii();
        }

        private void btnAdaugaFarmacie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FarmacieCurenta.Nume))
            {
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = "Numele farmaciei este obligatoriu!";
                return;
            }

            adminFarmacii.AdaugaFarmacie(FarmacieCurenta);
            lblStatus.Foreground = Brushes.Green;
            lblStatus.Content = "Farmacie adăugată cu succes!";

            FarmacieCurenta = new Farmacie(); // Golim casutele dupa adaugare
            AfiseazaFarmacii();
        }

        private void btnModificaFarmacie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FarmacieCurenta.Nume))
            {
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = "Selectează o farmacie pentru a o modifica!";
                return;
            }

            bool succes = adminFarmacii.ModificaFarmacie(FarmacieCurenta);
            if (succes)
            {
                lblStatus.Foreground = Brushes.Blue;
                lblStatus.Content = "Farmacie modificată cu succes!";
                AfiseazaFarmacii();
            }
            else
            {
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = "Farmacia nu a fost găsită (numele nu poate fi modificat)!";
            }
        }

        private void btnStergeFarmacie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FarmacieCurenta.Nume)) return;

            bool succes = adminFarmacii.StergeFarmacie(FarmacieCurenta.Nume);
            if (succes)
            {
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = "Farmacia a fost ștearsă!";
                FarmacieCurenta = new Farmacie();
                AfiseazaFarmacii();
            }
        }

        private void btnResetFarmacie_Click(object sender, RoutedEventArgs e)
        {
            FarmacieCurenta = new Farmacie();
            dgFarmacii.SelectedItem = null;
            lblStatus.Content = "";
        }

        private void dgFarmacii_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cand dam click pe un rand in tabel, incarcam datele direct in FarmacieCurenta.
            // Data Binding-ul va actualiza automat cele 3 TextBox-uri vizuale!
            if (dgFarmacii.SelectedItem is Farmacie farmacieSelectata)
            {
                // Facem o copie ca sa nu modificam direct randul din tabel pana nu dam click pe "Modifica"
                FarmacieCurenta = new Farmacie(farmacieSelectata.Nume, farmacieSelectata.Adresa, farmacieSelectata.Oras);
            }
        }

        
        // LOGICA MEDICAMENTE 
        
        private void btnSalveaza_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string denumire = txtDenumire.Text ?? "Fara Nume";
                double pret = Convert.ToDouble(txtPret.Text);
                int stoc = Convert.ToInt32(txtStoc.Text);

                FormaPrezentare forma = FormaPrezentare.Comprimate;
                if (rbSirop.IsChecked == true) forma = FormaPrezentare.Sirop;
                else if (rbUnguent.IsChecked == true) forma = FormaPrezentare.Unguent;
                else if (rbSolutie.IsChecked == true) forma = FormaPrezentare.SolutieInjectabila;

                ConditiiPastrare conditii = 0;
                if (chkTempCamerei.IsChecked == true) conditii |= ConditiiPastrare.TemperaturaCamerei;
                if (chkRefrigerare.IsChecked == true) conditii |= ConditiiPastrare.Refrigerare;
                if (chkCongelare.IsChecked == true) conditii |= ConditiiPastrare.Congelare;
                if (chkLumina.IsChecked == true) conditii |= ConditiiPastrare.FeritDeLumina;
                if (chkUmiditate.IsChecked == true) conditii |= ConditiiPastrare.FeritDeUmiditate;

                if (conditii == 0) conditii = ConditiiPastrare.TemperaturaCamerei;

                int nextId = adminMedicamente.GetStoc().Any() ? adminMedicamente.GetStoc().Max(m => m.IdMedicament) + 1 : 1;

                Medicament med = new Medicament(nextId, denumire, pret, stoc, forma, conditii);
                med.Categorie = lstCategorieAdaugare.SelectedItem?.ToString() ?? "Analgezice";
                med.DataExpirare = dtpDataExpirare.SelectedDate ?? DateTime.Today.AddYears(1);
                med.DataActualizare = DateTime.Now;

                adminMedicamente.AdaugaMedicament(med);

                lblStatus.Foreground = Brushes.Green;
                lblStatus.Content = "Medicament salvat cu succes!";

                txtDenumire.Clear(); txtPret.Clear(); txtStoc.Clear();
                AfiseazaInTabel(adminMedicamente.GetStoc());
            }
            catch (Exception)
            {
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = "Eroare la date! Asigurați-vă că prețul și stocul sunt numere valide.";
            }
        }

        private void cmbMedicamenteModificare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMedicamenteModificare.SelectedItem is Medicament med)
            {
                txtModificaDenumire.Text = med.Denumire;
                txtModificaPret.Text = med.Pret.ToString();
                txtModificaStoc.Text = med.CantitateStoc.ToString();
                dtpModificaDataExpirare.SelectedDate = med.DataExpirare;
            }
        }

        private void btnActualizeaza_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbMedicamenteModificare.SelectedItem is Medicament medSelectat)
                {
                    medSelectat.Denumire = txtModificaDenumire.Text ?? "Fara Nume";
                    medSelectat.Pret = Convert.ToDouble(txtModificaPret.Text);
                    medSelectat.CantitateStoc = Convert.ToInt32(txtModificaStoc.Text);
                    medSelectat.DataExpirare = dtpModificaDataExpirare.SelectedDate ?? DateTime.Today.AddYears(1);
                    medSelectat.DataActualizare = DateTime.Now;

                    bool succes = adminMedicamente.ModificaMedicament(medSelectat);

                    if (succes)
                    {
                        lblStatus.Foreground = Brushes.Blue;
                        lblStatus.Content = "Medicament modificat cu succes!";
                        AfiseazaInTabel(adminMedicamente.GetStoc());
                        cmbMedicamenteModificare.ItemsSource = null;
                        cmbMedicamenteModificare.ItemsSource = adminMedicamente.GetStoc();
                        txtModificaDenumire.Clear(); txtModificaPret.Clear(); txtModificaStoc.Clear();
                    }
                    else
                    {
                        lblStatus.Foreground = Brushes.Red;
                        lblStatus.Content = "Eroare la scrierea in fisier!";
                    }
                }
                else
                {
                    lblStatus.Foreground = Brushes.Orange;
                    lblStatus.Content = "Selectează un medicament mai întâi!";
                }
            }
            catch (Exception)
            {
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = "Eroare! Verificați dacă prețul și stocul sunt scrise corect.";
            }
        }

        private void btnExecutaCautare_Click(object sender, RoutedEventArgs e)
        {
            string denumireCautata = txtCautare.Text.Trim();

            if (string.IsNullOrEmpty(denumireCautata))
            {
                lblStatus.Foreground = Brushes.Orange;
                lblStatus.Content = "Introduceți un cuvânt pentru a căuta în arhiva uriașă!";
                return;
            }

            // Apelare modificata
            var rezultate = adminMedicamente.FiltreazaMedicamente(denumireCautata);

            AfiseazaInTabel(rezultate);

            if (rezultate.Count == 0)
            {
                lblStatus.Foreground = Brushes.Orange;
                lblStatus.Content = "Nu s-a găsit niciun medicament!";
            }
            else
            {
                lblStatus.Foreground = Brushes.Blue;
                lblStatus.Content = $"Căutare finalizată! S-au afișat {rezultate.Count} rezultate.";
            }
        }

        private void btnAfiseazaToti_Click(object sender, RoutedEventArgs e)
        {
            txtCautare.Clear();

            // Preluăm doar ultimele 10 adăugate
            var ultimele10 = adminMedicamente.GetUltimeleMedicamente(10);

            AfiseazaInTabel(ultimele10);

            lblStatus.Foreground = Brushes.Blue;
            lblStatus.Content = "Se afișează cele mai recente 10 medicamente";
        }

        private void AfiseazaInTabel(List<Medicament> lista)
        {
            dgMedicamente.ItemsSource = null;
            dgMedicamente.ItemsSource = lista;
        }
    }
}
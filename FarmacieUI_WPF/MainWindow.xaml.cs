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
    // =========================================================================
    // HEADER: CLASA PRINCIPALA A FERESTREI (MVVM + Paginare)
    // =========================================================================
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Aici definim interfețele de stocare
        private IStocareData adminMedicamente;
        private IStocareFarmacii adminFarmacii;

        // Aici sunt variabilele pentru controlul Paginării 
        private int paginaCurenta = 1;
        private int elementePePagina = 20;
        private List<Medicament> listaCurentaMedicamente = new List<Medicament>();

        // Aici reținem instanțele curente pentru formular
        private Farmacie farmacieCurenta;
        private Medicament medicamentCurent;

        // Aici este proprietatea Binding pentru Farmacii
        public Farmacie FarmacieCurenta
        {
            get => farmacieCurenta;
            set { farmacieCurenta = value; OnPropertyChanged(); }
        }

        // Aici este proprietatea Binding pentru Medicamente
        public Medicament MedicamentCurent
        {
            get => medicamentCurent;
            set { medicamentCurent = value; OnPropertyChanged(); }
        }

        // Aici este evenimentul de notificare a interfeței
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // =========================================================================
        // HEADER: CONSTRUCTOR
        // =========================================================================
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Aici inițializăm modulul de Medicamente
            adminMedicamente = new AdministrareMedicamenteFisierText("Medicamente.txt");
            MedicamentCurent = new Medicament();
            lstCategorieAdaugare.ItemsSource = Medicament.CategoriiDisponibile;
            lstCategorieAdaugare.SelectedIndex = 0;

            // Aici încărcăm tabelul de medicamente folosind Paginarea de la bun început
            AfiseazaInTabel(adminMedicamente.GetStoc());

            // Aici inițializăm modulul de Farmacii
            adminFarmacii = new AdministrareFarmaciiFisierText("Farmacii.txt");
            FarmacieCurenta = new Farmacie();
            AfiseazaFarmacii();
        }

        // =========================================================================
        // HEADER: MENIU DE NAVIGARE
        // =========================================================================

        private void btnMeniuAdauga_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouAdaugare.Visibility = Visibility.Visible;
            dgMedicamente.Visibility = Visibility.Visible;
            if (panouPaginare != null) panouPaginare.Visibility = Visibility.Visible;
        }

        private void btnMeniuModifica_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouModifica.Visibility = Visibility.Visible;
            dgMedicamente.Visibility = Visibility.Visible;
            if (panouPaginare != null) panouPaginare.Visibility = Visibility.Visible;

            // AM ȘTERS vechea încărcare a stocului complet de aici. 
            // ComboBox-ul este deja încărcat și sincronizat de funcția AfiseazaInTabel!

            txtModificaDenumire.Clear(); txtModificaPret.Clear(); txtModificaStoc.Clear();
            dtpModificaDataExpirare.SelectedDate = null;
        }

        private void btnMeniuCauta_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouCautare.Visibility = Visibility.Visible;
            dgMedicamente.Visibility = Visibility.Visible;
            if (panouPaginare != null) panouPaginare.Visibility = Visibility.Visible;
        }

        private void btnMeniuFarmacii_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanourile();
            panouFarmacii.Visibility = Visibility.Visible;
            dgMedicamente.Visibility = Visibility.Collapsed;
            if (panouPaginare != null) panouPaginare.Visibility = Visibility.Collapsed;

            if (cmbOrasFiltru != null) cmbOrasFiltru.SelectedIndex = 0;
            FarmacieCurenta = new Farmacie();
            AfiseazaFarmacii();
        }

        // Aici se ascund toate secțiunile active
        private void AscundeToatePanourile()
        {
            panouAdaugare.Visibility = Visibility.Collapsed;
            panouModifica.Visibility = Visibility.Collapsed;
            panouCautare.Visibility = Visibility.Collapsed;
            panouFarmacii.Visibility = Visibility.Collapsed;
            if (lblStatus != null) lblStatus.Content = "";
        }

        // =========================================================================
        // HEADER: GESTIUNE FARMACII
        // =========================================================================

        private void AfiseazaFarmacii()
        {
            if (dgFarmacii == null || adminFarmacii == null) return;
            var toateFarmaciile = adminFarmacii.GetFarmacii();
            string orasSelectat = cmbOrasFiltru?.SelectedItem is ComboBoxItem item ? item.Content.ToString() : "";

            if (orasSelectat == "Toate" || string.IsNullOrEmpty(orasSelectat))
                dgFarmacii.ItemsSource = toateFarmaciile;
            else
                dgFarmacii.ItemsSource = toateFarmaciile.Where(f => f.Oras.Equals(orasSelectat, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void cmbOrasFiltru_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AfiseazaFarmacii();
            FarmacieCurenta = new Farmacie();
            if (lblStatus != null) lblStatus.Content = "";
        }

        private void dgFarmacii_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFarmacii.SelectedItem is Farmacie farmacieSelectata)
            {
                FarmacieCurenta = new Farmacie(farmacieSelectata.Nume, farmacieSelectata.Adresa, farmacieSelectata.Oras, farmacieSelectata.Telefon, farmacieSelectata.Email);
                if (lblStatus != null) lblStatus.Content = "";
            }
        }

        private void btnAdaugaFarmacie_Click(object sender, RoutedEventArgs e)
        {
            adminFarmacii.AdaugaFarmacie(FarmacieCurenta);
            if (lblStatus != null) { lblStatus.Foreground = Brushes.Green; lblStatus.Content = "Farmacie adăugată cu succes!"; }
            FarmacieCurenta = new Farmacie();
            AfiseazaFarmacii();
        }

        private void btnModificaFarmacie_Click(object sender, RoutedEventArgs e)
        {
            if (adminFarmacii.ModificaFarmacie(FarmacieCurenta))
            {
                if (lblStatus != null) { lblStatus.Foreground = Brushes.Blue; lblStatus.Content = "Farmacie modificată cu succes!"; }
                AfiseazaFarmacii();
            }
            else if (lblStatus != null) { lblStatus.Foreground = Brushes.Red; lblStatus.Content = "Eroare! Numele este cheie primară și nu poate fi schimbat."; }
        }

        private void btnStergeFarmacie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FarmacieCurenta.Nume)) return;
            if (adminFarmacii.StergeFarmacie(FarmacieCurenta.Nume))
            {
                if (lblStatus != null) { lblStatus.Foreground = Brushes.Red; lblStatus.Content = "Farmacia a fost ștearsă din sistem!"; }
                FarmacieCurenta = new Farmacie();
                AfiseazaFarmacii();
            }
        }

        private void btnResetFarmacie_Click(object sender, RoutedEventArgs e)
        {
            FarmacieCurenta = new Farmacie();
            dgFarmacii.SelectedItem = null;
            if (lblStatus != null) lblStatus.Content = "";
        }

        // =========================================================================
        // HEADER: GESTIUNE MEDICAMENTE
        // =========================================================================

        // Aici preluăm datele și salvăm, funcția fiind securizată prin "IsEnabled" la Buton
        private void btnSalveaza_Click(object sender, RoutedEventArgs e)
        {
            FormaPrezentare forma = FormaPrezentare.Comprimate;
            if (rbSirop.IsChecked == true) forma = FormaPrezentare.Sirop;
            else if (rbUnguent.IsChecked == true) forma = FormaPrezentare.Unguent;
            else if (rbSolutie.IsChecked == true) forma = FormaPrezentare.SolutieInjectabila;
            MedicamentCurent.Forma = forma;

            ConditiiPastrare conditii = 0;
            if (chkTempCamerei.IsChecked == true) conditii |= ConditiiPastrare.TemperaturaCamerei;
            if (chkRefrigerare.IsChecked == true) conditii |= ConditiiPastrare.Refrigerare;
            if (chkCongelare.IsChecked == true) conditii |= ConditiiPastrare.Congelare;
            if (chkLumina.IsChecked == true) conditii |= ConditiiPastrare.FeritDeLumina;
            if (chkUmiditate.IsChecked == true) conditii |= ConditiiPastrare.FeritDeUmiditate;
            if (conditii == 0) conditii = ConditiiPastrare.TemperaturaCamerei;
            MedicamentCurent.Conditii = conditii;

            MedicamentCurent.Categorie = lstCategorieAdaugare.SelectedItem?.ToString() ?? "Analgezice";
            MedicamentCurent.IdMedicament = adminMedicamente.GetStoc().Any() ? adminMedicamente.GetStoc().Max(m => m.IdMedicament) + 1 : 1;
            MedicamentCurent.DataActualizare = DateTime.Now;

            adminMedicamente.AdaugaMedicament(MedicamentCurent);

            if (lblStatus != null) { lblStatus.Foreground = Brushes.Green; lblStatus.Content = "Medicament salvat cu succes!"; }

            rbComprimate.IsChecked = true;
            chkTempCamerei.IsChecked = false; chkRefrigerare.IsChecked = false;
            chkCongelare.IsChecked = false; chkLumina.IsChecked = false; chkUmiditate.IsChecked = false;

            MedicamentCurent = new Medicament();
            AfiseazaInTabel(adminMedicamente.GetStoc());
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

                    if (adminMedicamente.ModificaMedicament(medSelectat))
                    {
                        if (lblStatus != null) { lblStatus.Foreground = Brushes.Blue; lblStatus.Content = "Medicament modificat cu succes!"; }

                        // Reîncărcăm tabelul, dar RĂMÂNEM PE ACEEAȘI PAGINĂ (false)
                        AfiseazaInTabel(adminMedicamente.GetStoc(), false);

                        // AM ȘTERS manual reîncărcarea ComboBox-ului, se va face automat din funcția de mai sus
                        txtModificaDenumire.Clear(); txtModificaPret.Clear(); txtModificaStoc.Clear();
                    }
                }
            }
            catch (Exception)
            {
                if (lblStatus != null) { lblStatus.Foreground = Brushes.Red; lblStatus.Content = "Eroare! Verificați valorile numerice."; }
            }
        }

        private void btnExecutaCautare_Click(object sender, RoutedEventArgs e)
        {
            string denumireCautata = txtCautare.Text.Trim();
            if (string.IsNullOrEmpty(denumireCautata))
            {
                if (lblStatus != null) { lblStatus.Foreground = Brushes.Orange; lblStatus.Content = "Introduceți un cuvânt!"; }
                return;
            }
            AfiseazaInTabel(adminMedicamente.FiltreazaMedicamente(denumireCautata));
        }

        // Aici reafișăm tot stocul, delegând performanța sistemului de paginare
        private void btnAfiseazaToti_Click(object sender, RoutedEventArgs e)
        {
            txtCautare.Clear();
            AfiseazaInTabel(adminMedicamente.GetStoc());
            if (lblStatus != null) { lblStatus.Foreground = Brushes.Blue; lblStatus.Content = "Se afișează arhiva completă."; }
        }

        // =========================================================================
        // HEADER: SISTEM DE PAGINARE (LINQ Skip / Take)
        // =========================================================================

        
        // Aici este logica centrală care împarte lista mare în subliste de 20 elemente
        private void AfiseazaInTabel(List<Medicament> lista, bool reseteazaPagina = true)
        {
            if (reseteazaPagina) paginaCurenta = 1;
            listaCurentaMedicamente = lista;

            int totalPagini = (int)Math.Ceiling((double)listaCurentaMedicamente.Count / elementePePagina);
            if (totalPagini == 0) totalPagini = 1;

            // Aici decupăm folosind funcțiile LINQ: Skip (sare paginile trecute) și Take (ia 20 bucăți)
            var elementePagina = listaCurentaMedicamente
                                 .Skip((paginaCurenta - 1) * elementePePagina)
                                 .Take(elementePePagina)
                                 .ToList();

            dgMedicamente.ItemsSource = null;
            dgMedicamente.ItemsSource = elementePagina;

            // OPTIMIZARE : Sincronizăm ComboBox-ul cu elementele din pagina curentă a tabelului
            if (cmbMedicamenteModificare != null)
            {
                cmbMedicamenteModificare.ItemsSource = null;
                cmbMedicamenteModificare.ItemsSource = elementePagina;
            }

            if (lblPaginare != null)
                lblPaginare.Content = $"Pagina {paginaCurenta} din {totalPagini} (Total Stoc: {listaCurentaMedicamente.Count})";

            if (btnPaginaAnterioara != null)
                btnPaginaAnterioara.IsEnabled = paginaCurenta > 1;

            if (btnPaginaUrmatoare != null)
                btnPaginaUrmatoare.IsEnabled = paginaCurenta < totalPagini;
        }

        // Aici scădem indexul paginii și reîncărcăm tabelul
        private void btnPaginaAnterioara_Click(object sender, RoutedEventArgs e)
        {
            paginaCurenta--;
            AfiseazaInTabel(listaCurentaMedicamente, false);
        }

        // Aici creștem indexul paginii și reîncărcăm tabelul
        private void btnPaginaUrmatoare_Click(object sender, RoutedEventArgs e)
        {
            paginaCurenta++;
            AfiseazaInTabel(listaCurentaMedicamente, false);
        }
    }
}
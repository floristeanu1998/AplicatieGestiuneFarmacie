using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LibrarieModele;
using NivelStocareDate;

namespace FarmacieUI_WPF
{
    public partial class MainWindow : Window
    {
        private IStocareData adminMedicamente;

        public MainWindow()
        {
            InitializeComponent();
            adminMedicamente = new AdministrareMedicamenteFisierText("Medicamente.txt");

            // Incarcam datele in tabel la pornire
            AfiseazaInTabel(adminMedicamente.GetStoc());
        }

        // ================= MENIU VERTICAL =================
        private void btnMeniuAdauga_Click(object sender, RoutedEventArgs e)
        {
            panouAdaugare.Visibility = Visibility.Visible;
            panouCautare.Visibility = Visibility.Collapsed;
            lblStatus.Content = "";
        }

        private void btnMeniuCauta_Click(object sender, RoutedEventArgs e)
        {
            panouAdaugare.Visibility = Visibility.Collapsed;
            panouCautare.Visibility = Visibility.Visible;
            lblStatus.Content = "";
        }

        // ================= SALVARE MEDICAMENT =================
        private void btnSalveaza_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Preluare texte
                string denumire = txtDenumire.Text;
                double pret = Convert.ToDouble(txtPret.Text);
                int stoc = Convert.ToInt32(txtStoc.Text);

                // 2. Extragere valoare RadioButtons (Forma Prezentare)
                FormaPrezentare forma = FormaPrezentare.Comprimate; // Default
                if (rbSirop.IsChecked == true) forma = FormaPrezentare.Sirop;
                else if (rbUnguent.IsChecked == true) forma = FormaPrezentare.Unguent;
                else if (rbSolutie.IsChecked == true) forma = FormaPrezentare.SolutieInjectabila;

                // 3. Extragere valori CheckBoxes (Conditii Pastrare folosind operatii pe biti datorita [Flags])
                ConditiiPastrare conditii = 0; // Incepem de la 0
                if (chkTempCamerei.IsChecked == true) conditii |= ConditiiPastrare.TemperaturaCamerei;
                if (chkRefrigerare.IsChecked == true) conditii |= ConditiiPastrare.Refrigerare;
                if (chkCongelare.IsChecked == true) conditii |= ConditiiPastrare.Congelare;
                if (chkLumina.IsChecked == true) conditii |= ConditiiPastrare.FeritDeLumina;
                if (chkUmiditate.IsChecked == true) conditii |= ConditiiPastrare.FeritDeUmiditate;

                // Validare mica: Daca nu a bifat nimic, punem implicit Temp Camerei
                if (conditii == 0) conditii = ConditiiPastrare.TemperaturaCamerei;

                // 4. Generare ID simplu (cautam cel mai mare ID curent si adaugam 1)
                int nextId = adminMedicamente.GetStoc().Any() ? adminMedicamente.GetStoc().Max(m => m.IdMedicament) + 1 : 1;

                // 5. Creare obiect si salvare
                Medicament med = new Medicament(nextId, denumire, pret, stoc, forma, conditii);
                adminMedicamente.AdaugaMedicament(med);

                lblStatus.Foreground = Brushes.Green;
                lblStatus.Content = "Medicament salvat cu succes!";

                // Resetam casutele de text
                txtDenumire.Clear(); txtPret.Clear(); txtStoc.Clear();

                // Reimprospatam DataGrid-ul
                AfiseazaInTabel(adminMedicamente.GetStoc());
            }
            catch (Exception)
            {
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = "Eroare la date! Asigurați-vă că prețul și stocul sunt numere.";
            }
        }

        // CAUTARE 
        private void btnExecutaCautare_Click(object sender, RoutedEventArgs e)
        {
            string denumireCautata = txtCautare.Text.Trim();

            // Folosim LINQ pentru a cauta partial (Contains) in loc sa ceara numele complet fix
            var rezultate = adminMedicamente.GetStoc()
                .Where(m => m.Denumire.Contains(denumireCautata, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AfiseazaInTabel(rezultate);

            if (rezultate.Count == 0)
            {
                lblStatus.Foreground = Brushes.Orange;
                lblStatus.Content = "Nu s-a găsit niciun medicament!";
            }
            else
            {
                lblStatus.Foreground = Brushes.Blue;
                lblStatus.Content = $"S-au găsit {rezultate.Count} rezultate.";
            }
        }

        private void btnAfiseazaToti_Click(object sender, RoutedEventArgs e)
        {
            txtCautare.Clear();
            AfiseazaInTabel(adminMedicamente.GetStoc());
            lblStatus.Content = "";
        }

        // ================= HELPER =================
        private void AfiseazaInTabel(List<Medicament> lista)
        {
            dgMedicamente.ItemsSource = null;
            dgMedicamente.ItemsSource = lista;
        }
    }
}
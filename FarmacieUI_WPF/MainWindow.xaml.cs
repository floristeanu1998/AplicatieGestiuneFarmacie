using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibrarieModele;
using NivelStocareDate;

namespace NivelUIWPF  
{
    public partial class MainWindow : Window
    {
        private IStocareData adminMedicamente;

        public MainWindow()
        {
            InitializeComponent();

            
            adminMedicamente = new AdministrareMedicamenteFisierText("Medicamente.txt");

            AfiseazaInTabel(adminMedicamente.GetStoc());

            // Preluam Categoriile pentru ListBox-ul de adaugare
            lstCategorieAdaugare.ItemsSource = Medicament.CategoriiDisponibile;
            lstCategorieAdaugare.SelectedIndex = 0; // Selectam implicit prima categorie

            // Punem data implicita in calendar
            dtpDataExpirare.SelectedDate = DateTime.Today.AddYears(1);
        }

        //  MENIU VERTICAL 
        private void btnMeniuAdauga_Click(object sender, RoutedEventArgs e)
        {
            panouAdaugare.Visibility = Visibility.Visible;
            panouModifica.Visibility = Visibility.Collapsed;
            panouCautare.Visibility = Visibility.Collapsed;
            lblStatus.Content = "";
        }

        private void btnMeniuModifica_Click(object sender, RoutedEventArgs e)
        {
            panouAdaugare.Visibility = Visibility.Collapsed;
            panouModifica.Visibility = Visibility.Visible;
            panouCautare.Visibility = Visibility.Collapsed;
            lblStatus.Content = "";

            // Resetam ComboBox-ul pentru a incarca datele la zi
            cmbMedicamenteModificare.ItemsSource = null;
            cmbMedicamenteModificare.ItemsSource = adminMedicamente.GetStoc();

            // Curatam casutele vechi
            txtModificaDenumire.Clear();
            txtModificaPret.Clear();
            txtModificaStoc.Clear();
            dtpModificaDataExpirare.SelectedDate = null;
        }

        private void btnMeniuCauta_Click(object sender, RoutedEventArgs e)
        {
            panouAdaugare.Visibility = Visibility.Collapsed;
            panouModifica.Visibility = Visibility.Collapsed;
            panouCautare.Visibility = Visibility.Visible;
            lblStatus.Content = "";
        }

        //  ADAUGARE MEDICAMENT 
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

                //  Preluam noile date pentru Lab 9 
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

        //  MODIFICARE MEDICAMENT
        private void cmbMedicamenteModificare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMedicamenteModificare.SelectedItem is Medicament med)
            {
                // Cand alege un medicament din lista, ii completam automat casutele cu datele curente
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
                    // Preluam datele din casute
                    medSelectat.Denumire = txtModificaDenumire.Text ?? "Fara Nume";
                    medSelectat.Pret = Convert.ToDouble(txtModificaPret.Text);
                    medSelectat.CantitateStoc = Convert.ToInt32(txtModificaStoc.Text);
                    medSelectat.DataExpirare = dtpModificaDataExpirare.SelectedDate ?? DateTime.Today.AddYears(1);
                    medSelectat.DataActualizare = DateTime.Now; // Marcam ora exacta a actualizarii

                    // Apelam metoda noastra noua din backend
                    bool succes = adminMedicamente.ModificaMedicament(medSelectat);

                    if (succes)
                    {
                        lblStatus.Foreground = Brushes.Blue;
                        lblStatus.Content = "Medicament modificat cu succes!";
                        AfiseazaInTabel(adminMedicamente.GetStoc());

                        // Reimprospatam ComboBox-ul
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

        //  CAUTARE 
        private void btnExecutaCautare_Click(object sender, RoutedEventArgs e)
        {
            string denumireCautata = txtCautare.Text.Trim();

            var rezultate = adminMedicamente.GetStoc()
                .Where(m => (m.Denumire ?? string.Empty).Contains(denumireCautata, StringComparison.OrdinalIgnoreCase))
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

        //  HELPER
        private void AfiseazaInTabel(List<Medicament> lista)
        {
            dgMedicamente.ItemsSource = null;
            dgMedicamente.ItemsSource = lista;
        }
    }
}
using LibrarieModele;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FarmacieUI_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IncarcareDateMedicamentTest();
        }
        private void IncarcareDateMedicamentTest()
        {
            // Instatiem un medicament 
            Medicament medicamentTest=new Medicament(
                101,
                "Nurofen Raceala si Gripa",
                28.50,
                45,
                FormaPrezentare.Comprimate,
                ConditiiPastrare.TemperaturaCamerei | ConditiiPastrare.FeritDeLumina
                );

            // trimitem date catre interfata grafica
            // 2. Trimitem datele catre interfata grafica (proprietatile Label-urilor)
            lblDenumire.Content = $"[{medicamentTest.IdMedicament}] {medicamentTest.Denumire}";
            lblFormaConditii.Content = $"{medicamentTest.Forma} | {medicamentTest.Conditii}";
            lblPret.Content = $"{medicamentTest.Pret} RON";
            lblStoc.Content = $"{medicamentTest.CantitateStoc} buc.";

            // 3. Logica de prezentare bazata pe metoda  EsteDisponibil()
            if (medicamentTest.EsteDisponibil())
            {
                lblStatus.Content = "DISPONIBIL ÎN FARMACIE";
                lblStatus.Foreground = Brushes.Green;
                lblStoc.Foreground = Brushes.Black;
            }
            else
            {
                lblStatus.Content = "INDISPONIBIL (STOC EPUIZAT)";
                lblStatus.Foreground = Brushes.Red;
                lblStoc.Foreground = Brushes.Red; // Coloreaza stocul cu rosu ca avertizare
            }
        }
    }
}
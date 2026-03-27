using System.Configuration;
using System.IO;
using NivelStocareDate;

namespace AplicatieGestiuneFarmacie
{
    public static class StocareFactory
    {
        private const string FORMAT_SALVARE = "FormatSalvare";
        private const string NUME_FISIER = "NumeFisier";
        //Constanta pentru fisierul de farmacii
        private const string NUME_FISIER_FARMACII = "NumeFisierFarmacii";

        // Fabrica pentru Medicamente
        public static IStocareData GetAdministratorStocare()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT_SALVARE] ?? "";
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER] ?? "";

            string locatieFisierSolutie = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? "";
            string caleCompletaFisier = locatieFisierSolutie + "\\" + numeFisier;

            if (formatSalvare == "txt")
                return new AdministrareMedicamenteFisierText(caleCompletaFisier + "." + formatSalvare);
            else if (formatSalvare == "memorie")
                return new AdministrareStoc();

            return null;
        }

        //Fabrica pentru Farmacii
        public static IStocareFarmacii GetAdministratorFarmacii()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT_SALVARE] ?? "";
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER_FARMACII] ?? "";

            string locatieFisierSolutie = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? "";
            string caleCompletaFisier = locatieFisierSolutie + "\\" + numeFisier;

            if (formatSalvare == "txt")
            {
                return new AdministrareFarmaciiFisierText(caleCompletaFisier + "." + formatSalvare);
            }

            return null;
        }
    }
}
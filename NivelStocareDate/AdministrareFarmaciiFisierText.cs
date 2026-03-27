using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarieModele;

namespace NivelStocareDate
{
    public class AdministrareFarmaciiFisierText : IStocareFarmacii
    {
        private string numeFisier;

        public AdministrareFarmaciiFisierText(string numeFisier)
        {
            this.numeFisier = numeFisier;
            // Creeaza fisierul daca nu exista
            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AdaugaFarmacie(Farmacie farmacie)
        {
            // true = adaugam in continuare (append)
            using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, true))
            {
                streamWriterFisierText.WriteLine(farmacie.ConversieLaSirPentruFisier());
            }
        }

        public List<Farmacie> GetFarmacii()
        {
            List<Farmacie> farmacii = new List<Farmacie>();

            using (StreamReader streamReader = new StreamReader(numeFisier))
            {
                string linieFisier;
                while ((linieFisier = streamReader.ReadLine()) != null)
                {
                    // Evitam randurile goale din fisier
                    if (string.IsNullOrWhiteSpace(linieFisier))
                    {
                        continue;
                    }

                    Farmacie farmacie = new Farmacie(linieFisier);
                    farmacii.Add(farmacie);
                }
            }

            return farmacii;
        }

        public Farmacie CautaFarmacieDupaNume(string nume)
        {
            List<Farmacie> farmacii = GetFarmacii();
            return farmacii.FirstOrDefault(f => f.Nume.Equals(nume, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
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
        public bool ModificaFarmacie(Farmacie farmacieActualizata)
        {
            List<Farmacie> farmacii = GetFarmacii();
            bool gasit = false;

            using (StreamWriter sw = new StreamWriter(numeFisier, false))
            {
                foreach (Farmacie f in farmacii)
                {
                    if (f.Nume.Equals(farmacieActualizata.Nume, System.StringComparison.OrdinalIgnoreCase))
                    {
                        sw.WriteLine(farmacieActualizata.ConversieLaSirPentruFisier());
                        gasit = true;
                    }
                    else
                    {
                        sw.WriteLine(f.ConversieLaSirPentruFisier());
                    }
                }
            }
            return gasit;
        }

        public bool StergeFarmacie(string nume)
        {
            List<Farmacie> farmacii = GetFarmacii();
            int sterse = farmacii.RemoveAll(f => f.Nume.Equals(nume, System.StringComparison.OrdinalIgnoreCase));

            if (sterse > 0)
            {
                using (StreamWriter sw = new StreamWriter(numeFisier, false))
                {
                    foreach (Farmacie f in farmacii)
                    {
                        sw.WriteLine(f.ConversieLaSirPentruFisier());
                    }
                }
            }


            return sterse > 0;
        }

        
    }
}
using System;
using System.Collections.Generic;
using System.IO; // Pentru StreamWritrer si StreamReader
using System.Linq;

using LibrarieModele;

namespace NivelStocareDate
{
    //clasa semneaza contractul IStocareData
    public class AdministrareMedicamenteFisierText : IStocareData
    {
        private string numeFisier;

        
        public AdministrareMedicamenteFisierText(string numeFisier)  // Constructorul primeste numele fisierului in care vrem sa salvam
        {
            this.numeFisier = numeFisier;
            // Se incearca deschiderea fisierului in modul OpenOrCreate 
            // Astfel incat sa fie creat daca nu exista (il face gol) 
             Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate); 
             streamFisierText.Close(); 
        }

        public void AdaugaMedicament(Medicament med)
        {
            // Instructiunea 'using' va apela la final streamWriterFisierText.Close() automat! 
            // Al doilea parametru setat la 'true' indica modul 'append' (adauga in continuare, nu sterge ce era) 
             using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, true)) // true pentru scris in continuare la finalul fisierului text
            {
                // Apelam metoda  de conversie care separa datele medicamentului  cu ";"
                streamWriterFisierText.WriteLine(med.ConversieLaSirPentruFisier()); 
            }
        }

        public List<Medicament> GetStoc()
        {
            List<Medicament> medicamente = new List<Medicament>();

            using (StreamReader streamReader = new StreamReader(numeFisier))
            {
                string linieFisier;

                while ((linieFisier = streamReader.ReadLine()) != null)
                {
                    // Ignorăm rândurile goale sau spațiile din greșeală
                    if (string.IsNullOrWhiteSpace(linieFisier))
                    {
                        continue;
                    }

                    Medicament med = new Medicament(linieFisier);
                    medicamente.Add(med);
                }
            }

            return medicamente;
        }

        public Medicament CautaMedicamentDupaNume(string denumireCautata)
        {
            //  Citim tot din fisier
            List<Medicament> stoc = GetStoc();
            //  Cautam in lista
            return stoc.FirstOrDefault(m => m.Denumire.Equals(denumireCautata, System.StringComparison.OrdinalIgnoreCase));
        }

        public bool StergeMedicamentDupaNume(string nume)
        {
            
            //Citire stoc medicamente
            List<Medicament> stoc = GetStoc();

            //Stergem din lista extrasa
            int elementeSterse = stoc.RemoveAll(m => m.Denumire.Equals(nume, System.StringComparison.OrdinalIgnoreCase));

            // 3. Daca s-a sters ceva, luam lista noua si o SCRIEM PESTE fisierul vechi
            if (elementeSterse > 0)
            {
                // 'false' inseamna overwrite (sterge tot ce era si scrie de la 0)
                using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, false))
                {
                    foreach (Medicament med in stoc)
                    {
                        streamWriterFisierText.WriteLine(med.ConversieLaSirPentruFisier());
                    }
                }
            }

            return elementeSterse > 0;
        }

        public bool VanzareMedicament(string nume, int cantitateVanduta) 
        { 
            List<Medicament> stoc = GetStoc(); // Citim tot din fisier

            Medicament medGasit = stoc.FirstOrDefault(m => m.Denumire.Equals(nume, StringComparison.OrdinalIgnoreCase)); // Cautare medicament dupa nume 
            if (medGasit != null && medGasit.CantitateStoc >= cantitateVanduta) 
            { 
                medGasit.CantitateStoc -= cantitateVanduta; 
                // Rescriem tot fisierul cu noua cantitate 
                using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, false)) 
                { 
                    foreach (Medicament m in stoc) 
                    { 
                        streamWriterFisierText.WriteLine(m.ConversieLaSirPentruFisier()); 
                    } 
                } 
                return true; 
            } 
            return false;
        }
        public bool ModificaMedicament(Medicament medicamentActualizat)
        {
            // 1. Citim toate medicamentele curente din fisier
            List<Medicament> medicamente = GetStoc();
            bool actualizareCuSucces = false;

            // 2. Deschidem fisierul in modul 'overwrite' (parametrul 'false') pentru a-l suprascrie complet
            using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, false))
            {
                foreach (Medicament med in medicamente)
                {
                    Medicament medicamentPentruScrisInFisier = med;

                    // Daca gasim medicamentul pe care vrem sa il modificam (dupa ID), il inlocuim cu cel actualizat
                    if (med.IdMedicament == medicamentActualizat.IdMedicament)
                    {
                        medicamentPentruScrisInFisier = medicamentActualizat;
                    }

                    // 3. Scriem linia in fisier
                    streamWriterFisierText.WriteLine(medicamentPentruScrisInFisier.ConversieLaSirPentruFisier());
                }
                actualizareCuSucces = true;
            }

            return actualizareCuSucces;
        }
        public List<Medicament> GetUltimeleMedicamente(int nr)
        {
            // Folosim ReadLines  și luăm doar ultimele 'nr' linii din fișier
            // Această metodă este sigură pentru fișiere de milioane de rânduri
            var ultimeleLinii = File.ReadLines(numeFisier).TakeLast(nr);

            List<Medicament> rezultate = new List<Medicament>();
            foreach (var linie in ultimeleLinii)
            {
                if (!string.IsNullOrWhiteSpace(linie))
                {
                    rezultate.Add(new Medicament(linie));
                }
            }
            return rezultate;
        }

        public List<Medicament> FiltreazaMedicamente(string textCautat)
        {
            List<Medicament> rezultate = new List<Medicament>();
            string textCautatLower = textCautat.ToLower(); // Il facem cu litere mici ca sa gaseasca si "Nurofen" si "nurofen"

            // Deschidem fisierul
            using (StreamReader streamReader = new StreamReader(numeFisier))
            {
                string linieFisier;

                // Citim exact un singur rand la un moment dat (memoria  ramane goala)
                while ((linieFisier = streamReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linieFisier)) continue;

                    // Cautam cuvantul direct in linia bruta de text!
                    // Daca il contine, cream un obiect de tip Medicament.
                    if (linieFisier.ToLower().Contains(textCautatLower))
                    {
                        rezultate.Add(new Medicament(linieFisier));
                    }
                }
            }
            return rezultate;
        }



    }
}

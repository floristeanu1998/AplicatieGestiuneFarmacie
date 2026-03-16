using System;
//  Această linie este  pentru a putea folosi List<T>
using System.Collections.Generic;

namespace AplicatieGestiuneFarmacie
{
    public class AdministrareStoc
    {
        // Data membră privată
        private List<Medicament> stoc;

        // Constructorul
        public AdministrareStoc()
        {
            // Lista in loc de vectori pentru a manageria medicamentele, nu trebuie setata dimensiune
            stoc = new List<Medicament>();
        }

        // Metoda : Adăugarea unui medicament
        public void AdaugaMedicament(Medicament med)
        {
          
            stoc.Add(med);
            Console.WriteLine($"+ A fost adăugat medicamentul: {med.Denumire} (ID: {med.IdMedicament})");
        }

        // Metoda: Afișarea stocului
        public void AfiseazaStoc(Farmacie farmacie)
        {
            Console.WriteLine("\n==========================================");
            // Afișăm detaliile farmaciei folosind obiectul primit
            Console.WriteLine(farmacie.InfoFarmacie());
            Console.WriteLine("==========================================");

            // La liste e count in loc de length, se verifica daca e ceva in stoc
            if (stoc.Count == 0)
            {
                Console.WriteLine("Farmacia nu are niciun medicament pe stoc momentan.");
                return;
            }

            // Aici foreach pentru a trece automat prin toate elementele existente din stoc.
            foreach (Medicament med in stoc)
            {
                Console.WriteLine(med.Info());
            }
            Console.WriteLine("==========================================\n");
        }

        // Metoda : Căutare in stoc după denumire
        public Medicament CautaMedicamentDupaNume(string denumireCautata)
        {
            foreach (Medicament med in stoc)
            {
                
                if (med.Denumire.ToLower() == denumireCautata.ToLower())
                {
                    return med; // L-am găsit, îl returnăm imediat!
                }
            }

            return null; // Am ajuns la finalul listei și nu l-am găsit
        }
    }
}
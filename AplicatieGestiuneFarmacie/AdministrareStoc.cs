using System;
//  Această linie este  pentru a putea folosi List<T>
using System.Collections.Generic;
using System.Linq;

namespace AplicatieGestiuneFarmacie
{
    public class AdministrareStoc
    {
        private List<Medicament> stoc;
        public AdministrareStoc()
        {
            // Inițializăm lista de medicamente când creăm un manager de stoc.
            stoc = new List<Medicament>();
        }
        public void AdaugaMedicament(Medicament med)
        {
            stoc.Add(med);
           
        }

        public List<Medicament> GetStoc()
        {
            return new List<Medicament>(stoc);
        }
        public Medicament CautaMedicamentDupaNume(string DenumireCautata)
        {
            return stoc.FirstOrDefault(m => m.Denumire.Equals(DenumireCautata, StringComparison.OrdinalIgnoreCase));
        }
        public bool StergeMedicamentDupaNume(string nume)
        {
            int elementeSterse = stoc.RemoveAll(m => m.Denumire.Equals(nume, StringComparison.OrdinalIgnoreCase));
            return elementeSterse > 0; // Returnează true dacă s-a șters cel puțin un medicament
        }
    }
}
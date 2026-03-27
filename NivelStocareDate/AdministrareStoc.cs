using System;
using System.Collections.Generic;
using System.Linq;
using LibrarieModele;

namespace NivelStocareDate
{
    public class AdministrareStoc : IStocareData // Adaugat : IStocareData pentru implemetarea interfetei
    {
        private List<Medicament> stoc;

        public AdministrareStoc()
        {
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
            return elementeSterse > 0;
        }
    }
}
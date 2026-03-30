using System;
using System.Collections.Generic;
using LibrarieModele;

namespace NivelStocareDate
{
    public interface IStocareData // Doar se definesc denuimirile metodelor, fara implementare, pentru a putea avea mai multe implementari (ex: in fisier text, in baza de date, etc.)
    {
        void AdaugaMedicament(Medicament med);
        List<Medicament> GetStoc();
        Medicament CautaMedicamentDupaNume(string DenumireCautata);
        bool StergeMedicamentDupaNume(string nume);

        bool VanzareMedicament(string nume, int cantitateVanduta);
    }
}

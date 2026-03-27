using System;
using System.Collections.Generic;
using System.Text;

using LibrarieModele;

namespace NivelStocareDate
{
    public interface IStocareFarmacii
    {
        void AdaugaFarmacie(Farmacie farmacie);
        List<Farmacie> GetFarmacii();
        Farmacie CautaFarmacieDupaNume(string nume);

    }
}

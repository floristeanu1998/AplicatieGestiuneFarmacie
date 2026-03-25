using System;

namespace LibrarieModele
{
    public class Farmacie
    {
        public string Nume { get; set; }
        public string Adresa { get; set; }
        public string Oras { get; set; }

        public Farmacie(string nume, string adresa, string oras)
        {
            Nume = nume;
            Adresa = adresa;
            Oras = oras;
        }

        public string InfoFarmacie()
        {
            return $"Farmacia {Nume.ToUpper()}, situată în {Oras}, adresa: {Adresa}";
        }
    }
}
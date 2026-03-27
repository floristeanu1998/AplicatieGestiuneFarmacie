using System;

namespace LibrarieModele
{
    public class Farmacie
    {
        // Const pentru fisier
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';

        private const int NUME = 0;
        private const int ADRESA = 1;
        private const int ORAS = 2;

        public string Nume { get; set; }
        public string Adresa { get; set; }
        public string Oras { get; set; }




        public Farmacie(string nume, string adresa, string oras)
        {
            Nume = nume;
            Adresa = adresa;
            Oras = oras;
        }

        // Constructor pentru fisier
        public Farmacie(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);
            
                this.Nume = dateFisier[NUME];
                this.Adresa = dateFisier[ADRESA];
                this.Oras = dateFisier[ORAS];
            
        }

        public string InfoFarmacie()
        {
            return $"Farmacia {Nume.ToUpper()}, situată în {Oras}, adresa: {Adresa}";
        }

        public string ConversieLaSirPentruFisier()  // Metoda pentru salvare in fisier
        {
            string obiectFarmaciePentruFisier = string.Format("{1}{0}{2}{0}{3}",
                SEPARATOR_PRINCIPAL_FISIER, // {0}
                (Nume ?? "NECUNOSCUT"), // {1}
                (Adresa ?? "NECUNOSCUT"),// {2}
                (Oras ?? "NECUNOSCUT"));// {3}

            return obiectFarmaciePentruFisier;
        }
    }
}
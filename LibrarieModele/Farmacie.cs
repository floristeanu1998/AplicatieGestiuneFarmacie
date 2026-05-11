using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibrarieModele
{
    // Adaugam INotifyPropertyChanged
    public class Farmacie : INotifyPropertyChanged
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';
        private const int NUME = 0;
        private const int ADRESA = 1;
        private const int ORAS = 2;

        private string nume;
        private string adresa;
        private string oras;

        // Evenimentul pentru Binding
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Nume
        {
            get => nume;
            set { nume = value; OnPropertyChanged(); }
        }

        public string Adresa
        {
            get => adresa;
            set { adresa = value; OnPropertyChanged(); }
        }

        public string Oras
        {
            get => oras;
            set { oras = value; OnPropertyChanged(); }
        }

        // Constructori si metode
        public Farmacie() { } // Adauga un constructor gol util pentru adaugare

        public Farmacie(string nume, string adresa, string oras)
        {
            Nume = nume;
            Adresa = adresa;
            Oras = oras;
        }

        public Farmacie(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);
            if (dateFisier.Length >= 3)
            {
                this.Nume = dateFisier[NUME];
                this.Adresa = dateFisier[ADRESA];
                this.Oras = dateFisier[ORAS];
            }
        }

        public string InfoFarmacie()
        {
            return $"Farmacia {Nume?.ToUpper()}, situată în {Oras}, adresa: {Adresa}";
        }

        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}",
                SEPARATOR_PRINCIPAL_FISIER,
                (Nume ?? "NECUNOSCUT"),
                (Adresa ?? "NECUNOSCUT"),
                (Oras ?? "NECUNOSCUT"));
        }
    }
}
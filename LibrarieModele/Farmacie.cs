using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LibrarieModele
{
    // INotifyPropertyChanged: Permite actualizarea automata a UI-ului cand se schimba datele (Data Binding)
    // IDataErrorInfo: Permite validarea datelor si afisarea erorilor direct in controalele din XAML 
    public class Farmacie : INotifyPropertyChanged, IDataErrorInfo
    {
        // 1. CONSTANTE PENTRU FISIER
        // Definim pozitia fiecarui camp in fisierul text (ex: Nume;Adresa;Oras;Telefon;Email)
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';
        private const int NUME = 0;
        private const int ADRESA = 1;
        private const int ORAS = 2;
        private const int TELEFON = 3;
        private const int EMAIL = 4;

        // 2. VARIABILE PRIVATE (Starea interna)
        // Aici stocam efectiv datele in memorie. Le initializam cu string.Empty pentru a evita erorile de tip "null"
        private string nume = string.Empty;
        private string adresa = string.Empty;
        private string oras = string.Empty;
        private string telefon = string.Empty;
        private string email = string.Empty;

        // 3. EVENIMENTUL DE BINDING
        // Acesta este evenimentul care anunta interfata grafica (XAML) ca o valoare s-a modificat in cod
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 4. PROPRIETATI PUBLICE (Expunerea datelor catre UI)
        // Cand utilizatorul scrie in TextBox, se apeleaza 'set'. Acesta salveaza valoarea si striga OnPropertyChanged() 
        // pentru a notifica sistemul de Binding ca trebuie sa actualizeze si restul interfetei.
        public string Nume { get => nume; set { nume = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged("EsteValid"); } }
        public string Adresa { get => adresa; set { adresa = value ?? string.Empty; OnPropertyChanged(); } }
        public string Oras { get => oras; set { oras = value ?? string.Empty; OnPropertyChanged(); } }
        public string Telefon { get => telefon; set { telefon = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged("EsteValid"); } }
        public string Email { get => email; set { email = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged("EsteValid"); } }

        // 5. CONSTRUCTORI
        // Constructor implicit (gol) - folosit cand se da click pe "Adauga" pentru a avea un formular curat
        public Farmacie() { }

        // Constructor pentru compatibilitate cu codul existent
        public Farmacie(string nume, string adresa, string oras)
        {
            Nume = nume; Adresa = adresa; Oras = oras;
        }

        // Constructor complet - folosit cand preluam toate datele simultan
        public Farmacie(string nume, string adresa, string oras, string telefon, string email)
        {
            Nume = nume; Adresa = adresa; Oras = oras; Telefon = telefon; Email = email;
        }

        // Constructor pentru citirea din fisierul text
        // Primeste un rand intreg din fisier (ex: "Catena;Str. Mare;Suceava;0722123456;contact@catena.ro") 
        // si il sparge in bucati folosind separatorul ';'
        public Farmacie(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);
            if (dateFisier.Length >= 3)
            {
                this.Nume = dateFisier[NUME];
                this.Adresa = dateFisier[ADRESA];
                this.Oras = dateFisier[ORAS];
            }
            // Verificam daca fisierul are formatul nou (inclusiv telefon si email) pentru a nu crapa la fisiere vechi
            if (dateFisier.Length >= 5)
            {
                this.Telefon = dateFisier[TELEFON];
                this.Email = dateFisier[EMAIL];
            }
        }

        // 6. METODE UTILITARE
        // Returneaza un sir de caractere formatat frumos pentru afisare rapida (daca este nevoie in alte zone din aplicatie)
        public string InfoFarmacie()
        {
            return $"Farmacia {Nume.ToUpper()}, situată în {Oras}, adresa: {Adresa}";
        }

        // Pregateste obiectul pentru a fi scris inapoi in fisierul text, lipind proprietatile cu separatorul ';'
        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}",
                SEPARATOR_PRINCIPAL_FISIER,
                Nume, Adresa, Oras, Telefon, Email);
        }

        
        // 7. IMPLEMENTARE IDataErrorInfo (LOGICA DE VALIDARE)
        
        

        // Proprietate ceruta de interfata, dar de obicei nu este folosita in aplicatiile WPF moderne
        public string? Error => null;

        // Indexer-ul care verifica automat fiecare proprietate legata la interfata (TextBox)
        public string? this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Telefon):
                        if (string.IsNullOrWhiteSpace(Telefon))
                            return "Telefonul este obligatoriu!"; // Verificare camp gol
                        if (Telefon.Length != 10)
                            return "Telefonul trebuie să aibă exact 10 cifre!"; // Verificare lungime
                        if (!Telefon.All(char.IsDigit))
                            return "Telefonul trebuie să conțină doar cifre!"; // Verificare format cu LINQ
                        break;
                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email))
                            return "Email-ul este obligatoriu!";
                        if (!Email.Contains('@') || !Email.Contains('.'))
                            return "Introduceți o adresă de email validă!"; // Verificare format de baza email
                        break;
                    case nameof(Nume):
                        if (string.IsNullOrWhiteSpace(Nume))
                            return "Numele farmaciei nu poate fi gol!"; // Regula de baza pentru cheia principala
                        break;
                }
                // Daca ajungem aici si se returneaza null, inseamna ca nu exista nicio eroare pentru campul validat
                return null;
            }
        }

        // Proprietate de sinteza folosita in UI pentru a activa/dezactiva butonul de Salvare
        // Returneaza TRUE doar daca functia de validare de mai sus returneaza "null" (adica nicio eroare) pentru toate cele 3 campuri
        public bool EsteValid => string.IsNullOrEmpty(this[nameof(Telefon)]) &&
                                 string.IsNullOrEmpty(this[nameof(Email)]) &&
                                 string.IsNullOrEmpty(this[nameof(Nume)]);
    }
}
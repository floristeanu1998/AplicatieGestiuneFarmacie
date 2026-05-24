using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibrarieModele
{
    public enum FormaPrezentare // Enum simplu, o singură valoare posibilă.
    {
        Comprimate = 1,
        Sirop = 2,
        Unguent = 3,
        SolutieInjectabila = 4,
    }

    [Flags]
    public enum ConditiiPastrare // Enum cu Flags pentru a putea combina mai multe condiții
    {
        TemperaturaCamerei = 1,
        Refrigerare = 2,
        Congelare = 4,
        FeritDeLumina = 8,
        FeritDeUmiditate = 16
    }

    // Am adăugat INotifyPropertyChanged și IDataErrorInfo pentru MVVM și validare
    public class Medicament : INotifyPropertyChanged, IDataErrorInfo
    {
        
        // 1. CONSTANTE FIȘIER
        
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';
        private const int ID = 0;
        private const int DENUMIRE = 1;
        private const int PRET = 2;
        private const int CANTITATE = 3;
        private const int FORMA = 4;
        private const int CONDITII = 5;

        
        // 2. VARIABILE PRIVATE (Backing fields)
       
        private string denumire = string.Empty;
        private double pret;
        private int cantitateStoc;
        private FormaPrezentare forma = FormaPrezentare.Comprimate;
        private ConditiiPastrare conditii = ConditiiPastrare.TemperaturaCamerei;
        private string categorie = "Analgezice";
        private DateTime dataExpirare = DateTime.Today.AddYears(1);

        public int IdMedicament { get; set; }
        public DateTime DataActualizare { get; set; }

        public static readonly List<string> CategoriiDisponibile = new List<string>
        { "Analgezice", "Antibiotice", "Suplimente", "Cardiologice", "Dermatologice" };

        
        // 3. EVENIMENT BINDING (INotifyPropertyChanged)
       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        // 4. PROPRIETĂȚI PUBLICE (Cu notificare pentru UI)
        
        public string Denumire
        {
            get => denumire;
            set { denumire = value; OnPropertyChanged(); OnPropertyChanged("EsteValid"); }
        }

        public double Pret
        {
            get => pret;
            set { pret = value; OnPropertyChanged(); OnPropertyChanged("EsteValid"); }
        }

        public int CantitateStoc
        {
            get => cantitateStoc;
            set { cantitateStoc = value; OnPropertyChanged(); OnPropertyChanged("EsteValid"); }
        }

        public FormaPrezentare Forma
        {
            get => forma;
            set { forma = value; OnPropertyChanged(); }
        }

        public ConditiiPastrare Conditii
        {
            get => conditii;
            set { conditii = value; OnPropertyChanged(); }
        }

        public string Categorie
        {
            get => categorie;
            set { categorie = value; OnPropertyChanged(); }
        }

        public DateTime DataExpirare
        {
            get => dataExpirare;
            set { dataExpirare = value; OnPropertyChanged(); OnPropertyChanged("EsteValid"); }
        }

        // Proprietate computed pentru Afisare in ComboBox
        public string AfisareComboBox => $"{Denumire} (Stoc: {CantitateStoc})";
        public bool EsteDisponibil() => CantitateStoc > 0;

        
        // 5. CONSTRUCTORI
        
        public Medicament()
        {
            IdMedicament = 0;
            DataActualizare = DateTime.Now;
        }

        public Medicament(int id, string denumire, double pret, int cantitateStoc, FormaPrezentare forma, ConditiiPastrare conditii)
        {
            IdMedicament = id;
            Denumire = denumire;
            Pret = pret;
            CantitateStoc = cantitateStoc;
            Forma = forma;
            Conditii = conditii;
            DataActualizare = DateTime.Now;
        }

        public Medicament(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            this.IdMedicament = Convert.ToInt32(dateFisier[ID]);
            this.Denumire = dateFisier[DENUMIRE];
            this.Pret = Convert.ToDouble(dateFisier[PRET]);
            this.CantitateStoc = Convert.ToInt32(dateFisier[CANTITATE]);

            this.Forma = (FormaPrezentare)Convert.ToInt32(dateFisier[FORMA]);
            this.Conditii = (ConditiiPastrare)Convert.ToInt32(dateFisier[CONDITII]);

            if (dateFisier.Length > 6) this.Categorie = dateFisier[6];
            else this.Categorie = "Analgezice";

            if (dateFisier.Length > 7 && DateTime.TryParse(dateFisier[7], out DateTime dataExp))
                this.DataExpirare = dataExp;
            else this.DataExpirare = DateTime.Today.AddYears(1);

            if (dateFisier.Length > 8 && DateTime.TryParse(dateFisier[8], out DateTime dataAct))
                this.DataActualizare = dataAct;
            else this.DataActualizare = DateTime.Now;
        }

       
        // 6. METODE UTILITARE
        
        public string Info()
        {
            string status = EsteDisponibil() ? "Disponibil" : "Indisponibil";
            return $"[ID:{IdMedicament}][{status}] {Denumire} ({Forma}) |Conditii:{Conditii}| Pret: {Pret} RON | Stoc: {CantitateStoc} buc.";
        }

        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}",
                SEPARATOR_PRINCIPAL_FISIER, IdMedicament, Denumire, Pret, CantitateStoc,
                (int)Forma, (int)Conditii, Categorie,
                DataExpirare.ToString("dd/MM/yyyy"), DataActualizare.ToString("dd/MM/yyyy HH:mm:ss"));
        }

        
        // 7. VALIDARE DATELOR (IDataErrorInfo)
        
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Denumire):
                        if (string.IsNullOrWhiteSpace(Denumire)) return "Denumirea este obligatorie!";
                        break;
                    case nameof(Pret):
                        if (Pret <= 0) return "Prețul trebuie să fie mai mare ca 0!";
                        break;
                    case nameof(CantitateStoc):
                        if (CantitateStoc < 0) return "Stocul nu poate fi negativ!";
                        break;
                    case nameof(DataExpirare):
                        if (DataExpirare.Date < DateTime.Today) return "Data expirării nu poate fi în trecut!";
                        break;
                }
                return null;
            }
        }

        // Proprietate folosită pentru a activa/dezactiva automat butonul de Salvare
        public bool EsteValid => string.IsNullOrEmpty(this[nameof(Denumire)]) &&
                                 string.IsNullOrEmpty(this[nameof(Pret)]) &&
                                 string.IsNullOrEmpty(this[nameof(CantitateStoc)]) &&
                                 string.IsNullOrEmpty(this[nameof(DataExpirare)]);
    }
}
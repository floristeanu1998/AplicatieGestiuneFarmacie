using System;

namespace LibrarieModele
    {

    public enum FormaPrezentare // Enum simplu , o singura valoare posibila.
    {
        Comprimate=1,
        Sirop=2,
        Unguent=3,
        SolutieInjectabila=4,
    }
    [Flags]
    public enum ConditiiPastrare // Enum cu Flags pentru a putea combina mai multe conditii de pastrare
    {
        TemperaturaCamerei = 1,
        Refrigerare = 2,
        Congelare = 4,
        FeritDeLumina = 8,
        FeritDeUmiditate = 16
    }


    public class Medicament
    {
        // Constante fisier
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';

        private const int ID=0;
        private const int DENUMIRE = 1;
        private const int PRET = 2;
        private const int CANTITATE = 3;  // Indexam pozitiile datelor 
        private const int FORMA = 4;
        private const int CONDITII = 5;







        // Proprietati auto-implementare pentru a stoca informatii despre medicament
        public int IdMedicament { get; set; }
        public string Denumire { get; set; }
        public double Pret { get; set; }
        
        public int CantitateStoc { get; set; }

        public FormaPrezentare Forma { get; set; } // Proprietate pentru forma de prezentare a medicamentului
        public ConditiiPastrare Conditii { get; set; } // Proprietate pentru conditiile de pastrare a medicamentului

        public bool EsteDisponibil() => CantitateStoc > 0; // Proprietate computed pentru a verifica disponibilitatea medicamentului

        public Medicament()  // Constructor fara parametri peentru caz in care nu se ofera informatii la crearea obiectului si sa nu fie eroari daca incercam sa-l afisam
        {
            IdMedicament = 0;
            Denumire = string.Empty;
            Pret = 0.0; 
            CantitateStoc = 0;
            Forma = FormaPrezentare.Comprimate; // Valoare implicita pentru forma de prezentare
            Conditii = ConditiiPastrare.TemperaturaCamerei; // Valoare implicita pentru conditiile de pastrare
        }

        public Medicament(int id,string denumire, double pret, int cantitateStoc,FormaPrezentare forma, ConditiiPastrare conditii)  // Constructor cu parametri pentru a initializa proprietatile
        {
            Denumire = denumire;
            Pret = pret;
            CantitateStoc = cantitateStoc;
            IdMedicament= id;
            Forma = forma;
            Conditii = conditii;
        }

        // Constructor pentru fisier, primeste string din fisier si trebuie sa-l faca obiect
        public Medicament(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER); // Incarcam un vector cu dupa ce separam datele in fisier
            
            this.IdMedicament = Convert.ToInt32(dateFisier[ID]);
            this.Denumire = dateFisier[DENUMIRE];
            this.Pret = Convert.ToDouble(dateFisier[PRET]);
            this.CantitateStoc = Convert.ToInt32(dateFisier[CANTITATE]);


            this.Forma = (FormaPrezentare)Convert.ToInt32(dateFisier[FORMA]); // Convertim stringul din fisier in int si apoi in enum
            this.Conditii = (ConditiiPastrare)Convert.ToInt32(dateFisier[CONDITII]); 
        }



        public string Info() // Metoda pentru afisarea informatiilor despre medicament, inclusiv disponibilitatea
        {
            string status= EsteDisponibil() ? "Disponibil" : "Indisponibil";
            return $"[ID:{IdMedicament}][{status}] {Denumire} ({Forma}) |Conditii:{Conditii}| Pret: {Pret} RON | Stoc: {CantitateStoc} buc.";
        }


        // Metoda salvare in fisier 

        public string ConversieLaSirPentruFisier()
        {
            string obiectMedicamentPentruFisier = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}",


                SEPARATOR_PRINCIPAL_FISIER, // {0}
                IdMedicament.ToString(), // {1}
                Denumire , // {2}
                Pret.ToString(), // {3}
                CantitateStoc.ToString(), // {4}
                ((int)Forma).ToString(), // {5} - Convertim enum la int pentru a-l salva in fisier
                ((int)Conditii).ToString() // {6} - Convertim enum cu flags la int pentru a-l salva in fisier

                );
            return obiectMedicamentPentruFisier;
        }

    }
}

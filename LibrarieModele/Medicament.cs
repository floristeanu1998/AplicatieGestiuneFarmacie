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
        
        public string Info() // Metoda pentru afisarea informatiilor despre medicament, inclusiv disponibilitatea
        {
            string status= EsteDisponibil() ? "Disponibil" : "Indisponibil";
            return $"[ID:{IdMedicament}][{status}] {Denumire} ({Forma}) |Conditii:{Conditii}| Pret: {Pret} RON | Stoc: {CantitateStoc} buc.";
        }


    }
}

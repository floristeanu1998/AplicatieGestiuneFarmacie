using System;

namespace AplicatieGestiuneFarmacie
    {
    public class Medicament
    {
        // Proprietati auto-implementare pentru a stoca informatii despre medicament
        public string Denumire { get; set; }
        public double Pret { get; set; }
        
        public int CantitateStoc { get; set; }

        public bool EsteDisponibil() => CantitateStoc > 0; // Proprietate computed pentru a verifica disponibilitatea medicamentului

        public Medicament()  // Constructor fara parametri peentru caz in care nu se ofera informatii la crearea obiectului si sa nu fie eroari daca incercam sa-l afisam
        {
            Denumire = string.Empty;
            Pret = 0.0; 
            CantitateStoc = 0;
        }

        public Medicament(string denumire, double pret, int cantitateStoc)  // Constructor cu parametri pentru a initializa proprietatile
        {
            Denumire = denumire;
            Pret = pret;
            CantitateStoc = cantitateStoc;
        }
        
        public string Info() // Metoda pentru afisarea informatiilor despre medicament, inclusiv disponibilitatea
        {
            string status= EsteDisponibil() ? "Disponibil" : "Indisponibil";
            return $"[{status}] {Denumire} | Pret: {Pret} RON | Stoc: {CantitateStoc}";
        }


    }
}

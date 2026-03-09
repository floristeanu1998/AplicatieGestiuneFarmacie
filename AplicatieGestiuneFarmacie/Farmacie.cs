using System;

namespace AplicatieGestiuneFarmacie
{
    public class Farmacie
    {
        public string Nume { get; set; }

        private Medicament[] stocMedicamente; // Array pentru a stoca medicamentele disponibile in farmacie , e ca un raft
        
        private int numarMedicamenteAdaugate; // Variabila pentru a urmari numarul de medicamente adaugate in stoc, mai precis cate tipuri de medicamente distincte adaugam.

        public Farmacie(string nume, int capacitateMaxima) // Constructor
        {
            Nume = nume;
            stocMedicamente = new Medicament[capacitateMaxima]; // Initializam array-ul cu capacitatea specificata
            numarMedicamenteAdaugate = 0; // Initializam numarul de medicamente adaugate la 0
        }

        // Metode ale farmaciei: Adaugare medicament + Afisare stoc

        public void AdaugaMedicament(Medicament med)
        {
            if (numarMedicamenteAdaugate < stocMedicamente.Length) // Verificam daca mai avem loc in stoc
            {
                stocMedicamente[numarMedicamenteAdaugate] = med; // Adaugam medicamentul in stoc
                numarMedicamenteAdaugate++; // Incrementam numarul de tipuri de  medicamente adaugate
                Console.WriteLine($"+ A fost adaugat medicamentul: {med.Denumire} la pretul de {med.Pret} lei.");
            }
            else
            {
                Console.WriteLine($"EROARE:Stocul este plin. Nu se pot adauga mai multe medicamente:{med.Denumire}.");
            }
        }

        public void AfiseazaStoc()
        {
            Console.WriteLine($"\nStocul farmaciei {Nume.ToUpper()}:");

            if (numarMedicamenteAdaugate == 0)
            {
                Console.WriteLine("Farmacia nu are niciun medicament pe stoc momentan.");
                return;
            }

            for (int i =0; i < numarMedicamenteAdaugate; i++)
            {
                                Console.WriteLine(stocMedicamente[i].Info()); // Afisam informatiile despre fiecare medicament din stoc folosind metoda Info() din clasa Medicament

            }

        }

    }
}

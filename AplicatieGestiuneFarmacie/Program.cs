using System;

namespace AplicatieGestiuneFarmacie
{
    class Program
    {
        static void Main()
        {
            // 1. Creăm medicamentele (acum cu ID unic, conform cerinței)
            Medicament m1 = new Medicament(101, "Paracetamol", 15.5, 50);
            Medicament m2 = new Medicament(102, "Nurofen", 28.0, 30);
            Medicament m3 = new Medicament(103, "Aspirina", 12.0, 0); // Stoc 0
            Medicament m4 = new Medicament(104, "Vitamina C", 35.0, 100);

            // 2. Creăm două farmacii diferite
            Farmacie farmaciaBucuresti = new Farmacie("Catena", "Str. Mare 1", "Bucuresti");
            Farmacie farmaciaIasi = new Farmacie("Dr. Max", "Bd. Copou 5", "Iasi");

            // 3. Adăugăm medicamentele în stocul FIECĂREI farmacii în parte
            Console.WriteLine("--- Aprovizionare Farmacia Bucuresti ---");
            farmaciaBucuresti.ManagerStoc.AdaugaMedicament(m1);
            farmaciaBucuresti.ManagerStoc.AdaugaMedicament(m2);

            Console.WriteLine("\n--- Aprovizionare Farmacia Iasi ---");
            farmaciaIasi.ManagerStoc.AdaugaMedicament(m3);
            farmaciaIasi.ManagerStoc.AdaugaMedicament(m4);

            // 4. Afișăm stocurile individuale
            farmaciaBucuresti.ManagerStoc.AfiseazaStoc(farmaciaBucuresti);
            farmaciaIasi.ManagerStoc.AfiseazaStoc(farmaciaIasi);

            // 5. TESTĂM CĂUTAREA 
            Console.WriteLine("=== CAUTARE MEDICAMENT IN BUCURESTI ===");
            Console.Write("Introdu numele medicamentului pe care il cauti: ");
            string numeCautat = Console.ReadLine();

            // Apelăm metoda de căutare din managerul farmaciei din București
            Medicament rezultat = farmaciaBucuresti.ManagerStoc.CautaMedicamentDupaNume(numeCautat);

            if (rezultat != null)
            {
                Console.WriteLine("\nMedicament Gasit!");
                Console.WriteLine(rezultat.Info());
            }
            else
            {
                Console.WriteLine($"\nNe pare rau, dar {numeCautat} nu exista in stocul farmaciei din Bucuresti.");
            }

            Console.WriteLine("\nApasati orice tasta pentru a inchide...");
            Console.ReadKey();
        }
    }
}
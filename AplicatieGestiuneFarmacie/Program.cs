using System;

namespace AplicatieGestiuneFarmacie
{
    class Program
    {
        static void Main()
        {
            Farmacie Catena= new Farmacie("Catena",10); //Creare farmacie cu nume "Catena" si capacitate de 10 tipuri de medicamente

            // Creere obiecte de tip medicament

            Medicament med1= new Medicament("Paracetamol", 10.5, 100);
            Medicament med2= new Medicament("Nurofen", 15.0, 50);
            Medicament med3= new Medicament("Aspirina", 8.0, 0); // Medicament cu stoc 0 ca sa testam metoda EsteDisponibil()

            // Adaugare medicamente in farmacie

            Catena.AdaugaMedicament(med1);
            Catena.AdaugaMedicament(med2);
            Catena.AdaugaMedicament(med3);

            // Afisare stoc medicamente

            Catena.AfiseazaStoc();

            Console.WriteLine("Apasa orice tasta pentru a iesi...");
            Console.ReadKey();
        }


    }
}
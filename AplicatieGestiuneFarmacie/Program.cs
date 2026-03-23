using System;
using System.Collections.Generic;

namespace AplicatieGestiuneFarmacie
{
    class Program
    {
        static void Main()
        {
            // Inițializăm farmacia
            Farmacie farmaciaMea = new Farmacie("Catena", "Str. Stefan cel Mare 1", "Bucuresti");
            string optiune;

            do
            {
                Console.WriteLine("\n--- MENIU GESTIUNE FARMACIE ---");
                Console.WriteLine("C. Citire si adăugare medicament");
                Console.WriteLine("A. Afisare stoc complet");
                Console.WriteLine("F. Căutare medicament dupa nume");
                Console.WriteLine("E. Eliminare medicament dupa nume");
                Console.WriteLine("X. Iesire");
                Console.Write("Alegeti o optiune: ");

                optiune = Console.ReadLine()?.ToUpper() ?? string.Empty; // ?. -> daca e null, nu da eroare, ci returneaza null, iar ?? -> daca e null, returneaza string.Empty

                switch (optiune)
                {
                    case "C":
                        Medicament medNou = CitireMedicamentTastatura();
                        farmaciaMea.ManagerStoc.AdaugaMedicament(medNou);
                        Console.WriteLine("Medicament adăugat cu succes!");
                        break;

                    case "A":
                        List<Medicament> medicamente = farmaciaMea.ManagerStoc.GetStoc();
                        AfisareMedicamente(medicamente, farmaciaMea);
                        break;

                    case "F":
                        Console.Write("Introduceti numele cautat: ");
                        string nume = Console.ReadLine();
                        Medicament gasit = farmaciaMea.ManagerStoc.CautaMedicamentDupaNume(nume);
                        if (gasit != null)
                            Console.WriteLine(gasit.Info());
                        else
                            Console.WriteLine("Medicamentul nu a fost gasit.");
                        break;

                    case "E":
                        Console.Write("Introduceti numele medicamentului de eliminat: ");
                        string numeDeSters = Console.ReadLine();

                        bool succes= farmaciaMea.ManagerStoc.StergeMedicamentDupaNume(numeDeSters); // va fi true daca s-a sters, false daca nu s-a gasit
                        
                        if (succes)
                        {
                            Console.WriteLine($"Medicamentul '{numeDeSters}' a fost eliminat din stoc");
                        }
                        else
                        {
                            Console.WriteLine("Medicamentul nu a fost gasit, deci nu s-a sters nimic.");
                        }

                        break;

                    case "X":
                        Console.WriteLine("Inchidere program...");
                        break;

                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }
            } while (optiune != "X");
        }

        // METODĂ PENTRU CITIRE (Interfața cu utilizatorul)
        public static Medicament CitireMedicamentTastatura()
        {
            Console.Write("ID: ");
            int.TryParse(Console.ReadLine(), out int id);

            Console.Write("Denumire: ");
            string denumire = Console.ReadLine();

            Console.Write("Pret: ");
            double.TryParse(Console.ReadLine(), out double pret);

            Console.Write("Cantitate: ");
            int.TryParse(Console.ReadLine(), out int cantitate);

            return new Medicament(id, denumire, pret, cantitate);
        }

        // METODĂ PENTRU AFIȘARE (Interfața cu utilizatorul)
        public static void AfisareMedicamente(List<Medicament> medicamente, Farmacie farmacie)
        {
            Console.WriteLine($"\nSTOC PENTRU: {farmacie.InfoFarmacie()}");
            if (medicamente.Count == 0)
            {
                Console.WriteLine("Stocul este gol.");
            }
            else
            {
                foreach (var med in medicamente)
                {
                    Console.WriteLine(med.Info());
                }
            }
        }
    }
}
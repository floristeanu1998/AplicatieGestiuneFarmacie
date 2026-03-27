using System;
using System.Collections.Generic;
using LibrarieModele;
using NivelStocareDate;

namespace AplicatieGestiuneFarmacie
{
    class Program
    {
        static void Main()
        {
            // Cerem de la fabrica ambii manageri
            IStocareData managerStoc = StocareFactory.GetAdministratorStocare();
            IStocareFarmacii managerFarmacii = StocareFactory.GetAdministratorFarmacii();

            // Verificam daca avem farmacii salvate in fisier
            List<Farmacie> listaFarmacii = managerFarmacii.GetFarmacii();
            Farmacie farmaciaMea;

            if (listaFarmacii.Count == 0)
            {
                // Daca e prima data cand rulam si fisierul e gol, cream farmacia si o SALVAM
                farmaciaMea = new Farmacie("Catena", "Str. Stefan cel Mare 1", "Bucuresti");
                managerFarmacii.AdaugaFarmacie(farmaciaMea);
                Console.WriteLine("Farmacie noua salvata pe disc cu succes!");
            }
            else
            {
                // Daca exista deja in fisier, o incarcam pe prima!
                farmaciaMea = listaFarmacii[0];
                Console.WriteLine($"Farmacie incarcata din fisier: {farmaciaMea.Nume}");
            }
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

                optiune = Console.ReadLine()?.ToUpper() ?? string.Empty;

                switch (optiune)
                {
                    case "C":
                        Medicament medNou = CitireMedicamentTastatura();
                        managerStoc.AdaugaMedicament(medNou);
                        Console.WriteLine("Medicament adăugat cu succes!");
                        break;

                    case "A":
                        List<Medicament> medicamente = managerStoc.GetStoc();
                        AfisareMedicamente(medicamente, farmaciaMea);
                        break;

                    case "F":
                        Console.Write("Introduceti numele cautat: ");
                        string nume = Console.ReadLine();
                        Medicament gasit = managerStoc.CautaMedicamentDupaNume(nume);
                        if (gasit != null)
                            Console.WriteLine(gasit.Info());
                        else
                            Console.WriteLine("Medicamentul nu a fost gasit.");
                        break;

                    case "E":
                        Console.Write("Introduceti numele medicamentului de eliminat: ");
                        string numeDeSters = Console.ReadLine();

                        bool succes = managerStoc.StergeMedicamentDupaNume(numeDeSters);

                        if (succes)
                            Console.WriteLine($"Medicamentul '{numeDeSters}' a fost eliminat din stoc");
                        else
                            Console.WriteLine("Medicamentul nu a fost gasit, deci nu s-a sters nimic.");
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

            Console.WriteLine("\nAlegeti forma de prezentare:");
            foreach (int i in Enum.GetValues(typeof(FormaPrezentare)))
            {
                Console.WriteLine($"{i} - {(FormaPrezentare)i} ");
            }
            Console.Write("Optiune forma: ");
            int.TryParse(Console.ReadLine(), out int optForma);

            FormaPrezentare formaAlesa = (FormaPrezentare)optForma;
            if (!Enum.IsDefined(typeof(FormaPrezentare), formaAlesa))
            {
                formaAlesa = FormaPrezentare.Comprimate;
            }

            Console.WriteLine("\nAlegeti conditiile de pastrare:");
            Console.WriteLine("(Puteti alege mai multe introducand numerele separate prin virgula. Ex: 2, 8)");
            foreach (int i in Enum.GetValues(typeof(ConditiiPastrare)))
            {
                Console.WriteLine($"{i} - {(ConditiiPastrare)i}");
            }
            Console.Write("Optiuni conditii: ");
            string inputConditii = Console.ReadLine();

            ConditiiPastrare conditiiAlese = 0;
            string[] valoriIntroduse = inputConditii.Split(',');

            foreach (string val in valoriIntroduse)
            {
                if (int.TryParse(val.Trim(), out int optiuneConditie))
                {
                    conditiiAlese = conditiiAlese | (ConditiiPastrare)optiuneConditie;
                }
            }

            return new Medicament(id, denumire, pret, cantitate, formaAlesa, conditiiAlese);
        }

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
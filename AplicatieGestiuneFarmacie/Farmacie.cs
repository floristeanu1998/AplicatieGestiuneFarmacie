using System;

namespace AplicatieGestiuneFarmacie
{
    public class Farmacie
    {
        public string Nume { get; set; }
        public string Adresa { get; set; }
        public string Oras { get; set; }

        // Fiecare farmacie are propriul ei manager de stoc.
        public AdministrareStoc ManagerStoc { get; set; }

        // Constructor
        public Farmacie(string nume, string adresa, string oras)
        {
            Nume = nume;
            Adresa = adresa;
            Oras = oras;

            // Când deschidem o farmacie nouă creem si un depozit asociat.
            ManagerStoc = new AdministrareStoc();
        }

        public string InfoFarmacie()
        {
            return $"Farmacia {Nume.ToUpper()}, situată în {Oras}, adresa: {Adresa}";
        }
    }
}
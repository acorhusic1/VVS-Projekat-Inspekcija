using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konzolna_aplikacija_TODO_lista_.Klase;
using Konzolna_aplikacija_TODO_lista_.Servisi;

namespace Konzolna_aplikacija_TODO_lista_.Servisi
{
    public class ZadatakServis: IZadatakServis
    {
        public void UnesiZadatak(Korisnik korisnik, Zadatak zadatak)
        {
            korisnik.dodajZadatak(zadatak);
            var korisnikServis = new KorisnikServis();
            korisnikServis.AzurirajKorisnika(korisnik);
        }

        public void FiltrirajZadatke(List<Zadatak> zadaci, int opcija)
        {
           List<Zadatak> filtriraniZadaci=new List<Zadatak>();
            switch (opcija)
            {
                case 1:
                    Console.WriteLine("Odaberite kategoriju: ");
                    Console.WriteLine("1 - LIČNI");
                    Console.WriteLine("2 - POSLOVNI");
                    Console.WriteLine("3 - OBRAZOVNI");
                    var kategorijaOpcija = Console.ReadLine();
                    Kategorija kategorijaFilter = kategorijaOpcija == "1" ? Kategorija.LIČNI :
                                                  kategorijaOpcija == "2" ? Kategorija.POSLOVNI :
                                                  Kategorija.OBRAZOVNI;
                    filtriraniZadaci = zadaci.Where(z => z.kategorija == kategorijaFilter).ToList();
                    break;
                case 2:
                    Console.WriteLine("Odaberite status: ");
                    Console.WriteLine("1 - U ČEKANJU");
                    Console.WriteLine("2 - U TOKU");
                    Console.WriteLine("3 - ZAVRŠEN");
                    var statusOpcija = Console.ReadLine();
                    Status statusFilter = statusOpcija == "1" ? Status.U_ČEKANJU :
                                          statusOpcija == "2" ? Status.U_TOKU :
                                          Status.ZAVRŠEN;
                    filtriraniZadaci = zadaci.Where(z => z.status == statusFilter).ToList();
                    break;
                case 3:
                    Console.WriteLine("Odaberite prioritet: ");
                    Console.WriteLine("1 - NIZAK");
                    Console.WriteLine("2 - SREDNJI");
                    Console.WriteLine("3 - VISOK");
                    var prioritetOpcija = Console.ReadLine();
                    Prioritet prioritetFilter = prioritetOpcija == "1" ? Prioritet.NIZAK :
                                                prioritetOpcija == "2" ? Prioritet.SREDNJI :
                                                Prioritet.VISOK;
                    filtriraniZadaci = zadaci.Where(z => z.prioritet == prioritetFilter).ToList();
                    break;

                default:
                    Console.WriteLine("Nepostojeća opcija za filtriranje!");
                    return;
            }
            Console.WriteLine("Filtrirani zadaci po kriteriju: ");
            foreach(var zadatak in filtriraniZadaci)
            {
                zadatak.ProvjeriRok();
                Console.WriteLine($"Opis: {zadatak.opis}, Kategorija: {zadatak.kategorija}, Status: {zadatak.status}, Prioritet: {zadatak.prioritet}, Rok završetka: {zadatak.rokZavrsetka}");
            }
        }

        public void StatistikaListe(List<Zadatak> zadaci)
        {
            foreach (var zadatak in zadaci)
            {
                zadatak.ProvjeriRok();
            }
            int brojZavrsenih = zadaci.Count(z => z.status == Status.ZAVRŠEN);
            int brojAktivnih = zadaci.Count(z => z.status == Status.U_TOKU);
            int brojOcekivanih = zadaci.Count(z => z.status == Status.U_ČEKANJU);
            int brojOdlozenih = zadaci.Count(z => z.status == Status.ODLOŽEN);

            // Prosječno trajanje izvršenja završenih zadataka
            var zavrseniZadaci = zadaci.Where(z => z.status == Status.ZAVRŠEN && z.vrijemeZavrsetka != null && z.vrijemePocetka != null).ToList();

            TimeSpan ukupnoVrijeme = TimeSpan.Zero;
            foreach (var zadatak in zavrseniZadaci)
            {
                ukupnoVrijeme += zadatak.vrijemeZavrsetka.Value - zadatak.vrijemePocetka.Value;
            }

            double prosjecnoVrijemeIzvrsenja = zavrseniZadaci.Count > 0 ? ukupnoVrijeme.TotalMinutes / zavrseniZadaci.Count : 0;

            // Ispis statistike
            Console.WriteLine("Statistika to-do liste:");
            Console.WriteLine($"Završeni zadaci: {brojZavrsenih}");
            Console.WriteLine($"Aktivni zadaci: {brojAktivnih}");
            Console.WriteLine($"Zadaci u čekanju: {brojOcekivanih}");
            Console.WriteLine($"Odloženi zadaci: {brojOdlozenih}");
            if(brojZavrsenih==0) Console.WriteLine($"Prosječno vrijeme izvršavanja završenih zadataka: n/A minuta");
            else
            Console.WriteLine($"Prosječno vrijeme izvršavanja završenih zadataka: {prosjecnoVrijemeIzvrsenja} minuta");
        }

        public void IzlistajSvePodsjetnike(Korisnik korisnik)
        {
            if (korisnik.listaPodsjetnika.Count == 0)
            {
                Console.WriteLine("Nemate nijedan podsjetnik.");
            }
            else
            {
                Console.WriteLine("Lista svih podsjetnika:");
                foreach (var podsjetnik in korisnik.listaPodsjetnika)
                {
                    Console.WriteLine($"Zadatak: {podsjetnik.zadatak.opis}, Podsjetnik postavljen za: {podsjetnik.vrijemeSlanja}, Izvršen: {podsjetnik.izvrsen}");
                }
            }
        }
    }
}

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Konzolna_aplikacija_TODO_lista_.Klase;
using Konzolna_aplikacija_TODO_lista_.Servisi;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Konzolna_aplikacija_TODO_lista_
{
    class Program
    {
        private static Timer _timer;
        static void Main(String[] args)
        {
            var korisnikServis = new KorisnikServis();
            var zadatakServis = new ZadatakServis();
            Korisnik prijavljeniKorisnik = null;

            Console.WriteLine("Dobrodošli u TaskMasters aplikaciju!");

            _timer = new Timer(120000);// provjeravanje svako 2 minute
            _timer.Elapsed += (sender, e) => ProvjeraRokova(zadatakServis,prijavljeniKorisnik,korisnikServis);
            _timer.AutoReset = true;
            _timer.Enabled = true;

            //ovdje cu ucitat korisnike iz baze koji su vec nekad bili spaseni
            //korisnikServis.ObrisiSveIzJsonFajla();
            /*var korisnici = korisnikServis.getKorisnici();
            foreach (var korisnik in korisnici)
            {
                Console.WriteLine(korisnik.korisnickoIme);
                Console.WriteLine(korisnik.lozinka);
            }*/
            while (true)
            {
                if (prijavljeniKorisnik == null)
                {
                    Console.WriteLine("Opcije: ");
                    Console.WriteLine("1 - Registracija korisnika");
                    Console.WriteLine("2 - Prijava korisnika");
                    Console.WriteLine("3 - Izlaz");
                    Console.Write("Izaberite opciju: ");

                    var izbor = Console.ReadLine();
                    switch (izbor)
                    {
                        case "1":
                            Console.Write("Unesite korisnicko ime: ");
                            var korisnickoImetemp=Console.ReadLine();
                            Console.Write("Unesite lozinku: ");
                            var lozinkaTemp = Console.ReadLine();
                            Console.Write("Unesite email: ");
                            var emailTemp = Console.ReadLine();
                            Korisnik korisnikTemp = null;
                            try
                            {
                                korisnikServis.VecPostojiKorisnik(korisnickoImetemp);
                                korisnikTemp = new Korisnik(korisnickoImetemp, emailTemp, lozinkaTemp, new List<Zadatak>(),new List<Podsjetnik>());
                                korisnikServis.RegistrujKorisnik(korisnikTemp);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex);
                            }
                            break;
                        case "2":
                            Console.Write("Unesite korisnicko ime: ");
                            korisnickoImetemp = Console.ReadLine();
                            Console.Write("Unesite lozinku: ");
                            lozinkaTemp=Console.ReadLine();
                            prijavljeniKorisnik = korisnikServis.Prijava(korisnickoImetemp, lozinkaTemp);
                            if (prijavljeniKorisnik != null)
                            {
                                Console.WriteLine("Uspješna prijava!");
                            }
                            else
                            {
                                Console.WriteLine("Neispravno korisničko ime ili lozinka");
                            }
                            break;
                        case "3": return;
                        default:
                            Console.WriteLine("Pogrešan unos!");
                            break;
                    }
                }else
                {
                    //ako je korisnik prijavljen onda imamo ostale opcije
                    Console.WriteLine("Opcije: ");
                    Console.WriteLine("1 - Unos zadatka");
                    Console.WriteLine("2 - Postavljanje podsjetnika");
                    Console.WriteLine("3 - Filtriranje zadataka");
                    Console.WriteLine("4 - Statistika to-do liste");
                    Console.WriteLine("5 - Označi zadatak kao započet");
                    Console.WriteLine("6 - Označi zadatak kao završen");
                    Console.WriteLine("7 - Izlistaj sve podsjetnike");
                    Console.WriteLine("8 - Odjavi se");
                    Console.Write("Izaberite opciju: ");
                    var opcija2= Console.ReadLine();
                    switch (opcija2) {
                        case "1":
                            Console.Write("Unesite opis zadatka: ");
                            var opisTemp= Console.ReadLine();
                            Console.Write("Unesite kategoriju (1 za LIČNI, 2 za POSLOVNI i 3 za OBRAZOVNI): ");
                            var opcijakategorija=Console.ReadLine();
                            var kategorijaTemp = opcijakategorija == "1" ? Kategorija.LIČNI : (opcijakategorija == "2" ? Kategorija.POSLOVNI : Kategorija.OBRAZOVNI);
                            Console.Write("Unesite prioritet (1 za NIZAK, 2 za SREDNJI i 3 za VISOK): ");
                            var opcijaprioritet= Console.ReadLine();
                            var prioritetTemp = opcijaprioritet == "1" ? Prioritet.NIZAK : (opcijaprioritet == "2" ? Prioritet.SREDNJI : Prioritet.VISOK);
                            Console.Write("Unesite rok završetka (koristiti format yyyy-mm-dd HH:mm): ");
                            if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datumTemp))
                            {
                                Zadatak noviZadatak = null;
                                try
                                {
                                    noviZadatak = new Zadatak(opisTemp, kategorijaTemp, Status.U_ČEKANJU, prioritetTemp, null, datumTemp, null);
                                    zadatakServis.UnesiZadatak(prijavljeniKorisnik, noviZadatak);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine("Neispravan format datuma i vremena!");
                            }
                            break;
                        case "2":
                            if (prijavljeniKorisnik.toDoLista.Count == 0)
                            {
                                Console.WriteLine("Korisnik nema nikakvih unesenih zadataka!");
                            }
                            else
                            {
                                Console.WriteLine("Odaberite za koji zadatak se dodaje podsjetnik");
                                for(int i=0; i<prijavljeniKorisnik.toDoLista.Count; i++)
                                {
                                    var zadatak = prijavljeniKorisnik.toDoLista[i];
                                    Console.WriteLine($"{i + 1}. {zadatak.opis} - Rok završetka: {zadatak.rokZavrsetka}");
                                }
                                Console.Write("Unesite broj zadatka: ");
                                if (int.TryParse(Console.ReadLine(), out int brojZadatka) && brojZadatka > 0 && brojZadatka <= prijavljeniKorisnik.toDoLista.Count)
                                {
                                    var odabraniZadatak = prijavljeniKorisnik.toDoLista[brojZadatka - 1];

                                    Console.Write("Unesite datum i vrijeme podsjetnika (format: yyyy-MM-dd HH:mm): ");
                                    if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime vrijemePodsjetnika))
                                    {
                                        try
                                        {
                                            var podsjetnikTemp = new Podsjetnik(vrijemePodsjetnika, odabraniZadatak, false,true);
                                            Console.WriteLine($"Podsjetnik je postavljen za zadatak: {odabraniZadatak.opis} na {vrijemePodsjetnika}");
                                            prijavljeniKorisnik.dodajPodsjetnik(podsjetnikTemp);
                                            korisnikServis.AzurirajKorisnika(prijavljeniKorisnik);
                                        }catch(Exception ex)
                                        {
                                            Console.WriteLine(ex.ToString());
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Neispravan format datuma i vremena.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Neispravan broj zadatka.");
                                }
                            }
                            break;
                        case "3":
                            if (prijavljeniKorisnik.toDoLista.Count == 0)
                            {
                                Console.WriteLine("Korisnik nema nikakvih unesenih zadataka!");
                            }
                            else
                            {
                                Console.WriteLine("Odaberite način filtriranja to-do liste: ");
                                Console.WriteLine("1 - Filtriranje po kategoriji");
                                Console.WriteLine("2 - Filtriranje po statusu");
                                Console.WriteLine("3 - Filtriranje po prioritetu");
                                Console.Write("Odaberite opciju: ");
                                var opcija3 = Console.ReadLine();
                                zadatakServis.FiltrirajZadatke(prijavljeniKorisnik.toDoLista, int.Parse(opcija3));
                            }
                            break;
                        case "4":
                            //statistika to do liste 
                            zadatakServis.StatistikaListe(prijavljeniKorisnik.toDoLista);
                            break;
                        case "5":
                            //oznacavanje zadatka kao zapocetog
                            if (prijavljeniKorisnik.toDoLista.Count == 0)
                            {
                                Console.WriteLine("Korisnik nema nikakvih unesenih zadataka!");
                            }
                            else
                            {
                                Console.WriteLine("Odaberite za koji zadatak se postavlja da je započet");
                                for (int i = 0; i < prijavljeniKorisnik.toDoLista.Count; i++)
                                {
                                    var zadatak = prijavljeniKorisnik.toDoLista[i];
                                    Console.WriteLine($"{i + 1}. {zadatak.opis} - Rok završetka: {zadatak.rokZavrsetka}");
                                }
                                Console.Write("Unesite broj zadatka: ");
                                var brojZadatka2= Console.ReadLine();
                                //linija var odabraniZadatak2 pomjerena u try ako pokrenemo zadatak pod rednim brojem koji NE POSTOJI
                                try
                                {
                                    var odabraniZadatak2 = prijavljeniKorisnik.toDoLista[int.Parse(brojZadatka2) - 1];
                                    odabraniZadatak2.ZapocniZadatak();
                                    korisnikServis.AzurirajKorisnika(prijavljeniKorisnik);
                                }
                                catch (ArgumentOutOfRangeException ex) 
                                {
                                    Console.WriteLine("Ne postoji zadatak pod tim rednim brojem!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                                break;
                        case "6":
                            if (prijavljeniKorisnik.toDoLista.Count == 0)
                            {
                                Console.WriteLine("Korisnik nema nikakvih unesenih zadataka!");
                            }
                            else
                            {
                                Console.WriteLine("Odaberite za koji zadatak se postavlja da je završen");
                                for (int i = 0; i < prijavljeniKorisnik.toDoLista.Count; i++)
                                {
                                    var zadatak = prijavljeniKorisnik.toDoLista[i];
                                    Console.WriteLine($"{i + 1}. {zadatak.opis} - Rok završetka: {zadatak.rokZavrsetka}");
                                }
                                Console.Write("Unesite broj zadatka: ");
                                var brojZadatka2 = Console.ReadLine();
                                try
                                {
                                    var odabraniZadatak2 = prijavljeniKorisnik.toDoLista[int.Parse(brojZadatka2) - 1];
                                    odabraniZadatak2.ZavrsiZadatak();
                                    korisnikServis.AzurirajKorisnika(prijavljeniKorisnik);
                                }
                                catch (ArgumentOutOfRangeException ex) 
                                {
                                    Console.WriteLine("Ne postoji zadatak pod tim rednim brojem!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            break;
                        case "7":
                            zadatakServis.IzlistajSvePodsjetnike(prijavljeniKorisnik);break;
                        case "8":
                            Console.WriteLine($"Odjavljeni ste sa profila {prijavljeniKorisnik.korisnickoIme}");
                            prijavljeniKorisnik = null;
                            break;
                        //
                        default:
                            Console.WriteLine("Pogrešan unos!");
                            break; 
                    }
                }
            }

        }

        private static void ProvjeraRokova(ZadatakServis zadatakServis,Korisnik prijavljeniKorisnik, KorisnikServis korisnikServis)
        {
            if (prijavljeniKorisnik != null)
            {
                int brojPromijenjenih=prijavljeniKorisnik.provjeriRokoveZadataka();
                //Console.WriteLine("Provjera rokova završena i korisnik ažuriran.");
                //provjera za podsjetnike
                var brojPromijenjenihPodsjetnika = 0;
                if (prijavljeniKorisnik.listaPodsjetnika.Count > 0) //ako ima elemenata tjst podsjetnika
                {
                    foreach (var podsjetnik in prijavljeniKorisnik.listaPodsjetnika)
                    {
                        if (podsjetnik.ispisPodsjetnika()) brojPromijenjenihPodsjetnika++;
                    }
                }
                if(brojPromijenjenih>0 || brojPromijenjenihPodsjetnika>0)
                korisnikServis.AzurirajKorisnika(prijavljeniKorisnik,true);
            }
            else
            {
                //Console.WriteLine("Nema prijavljenog korisnika za provjeru.");
            }
        }

    }
}

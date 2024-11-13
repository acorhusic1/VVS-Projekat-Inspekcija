using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Konzolna_aplikacija_TODO_lista_.Klase;
using Newtonsoft.Json;

namespace Konzolna_aplikacija_TODO_lista_.Servisi
{
    public class KorisnikServis: IKorisnikServis
    {
        private readonly string putanjafilea = Path.Combine(Environment.CurrentDirectory, "Podaci", "korisnici.json");

        public List<Korisnik> getKorisnici()
        {
            try
            {
                if (File.Exists(putanjafilea))
                {
                    var json = File.ReadAllText(putanjafilea);
                    return JsonConvert.DeserializeObject<List<Korisnik>>(json) ?? new List<Korisnik>();
                }
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Greška pri deserijalizaciji JSON-a: {ex.Message}");
                return new List<Korisnik>(); // U slučaju greške, kreiraj praznu listu
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Greška pri čitanju fajla: {ex.Message}");
                return new List<Korisnik>(); // U slučaju greške, kreiraj praznu listu
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Nepredviđena greška: {ex.Message}");
                return new List<Korisnik>(); // U slučaju nepredviđene greške, kreiraj praznu listu
            }
            return new List<Korisnik>();
        }

        public void SpremiKorisnike(List<Korisnik> korisnici)
        {
            try
            {
                var json = JsonConvert.SerializeObject(korisnici, Formatting.Indented);
                File.WriteAllText(putanjafilea, json);
            }catch(Exception ex)
            {
                Console.WriteLine("Greska pri upisu u fajl: ", ex);
            }
        }

        public void RegistrujKorisnik(Korisnik korisnik)
        {
            var korisnici=getKorisnici();
            korisnik.lozinka = Enkripcija(korisnik.lozinka);
            korisnici.Add(korisnik);
            SpremiKorisnike(korisnici);
            Console.WriteLine("Registracija je uspješna!");
        }

        public Korisnik Prijava(string korisnickoIme, String lozinka)
        {
            var korisnici = getKorisnici();
            //lozinka= lozinka.Trim();
            var korisnik = korisnici.FirstOrDefault(u => u.korisnickoIme == korisnickoIme);
            if (korisnik != null && korisnik.lozinka == Enkripcija(lozinka))
                return korisnik;
            return null;
        }

        private String Enkripcija(String lozinka)
        {
            //lozinka = lozinka.Trim();
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(lozinka));
            return Convert.ToBase64String(bytes);
        }

        public void ObrisiSveIzJsonFajla()
        {
            File.WriteAllText(putanjafilea, "[]");
            Console.WriteLine("Svi podaci su obrisani iz JSON fajla!");
        }

        public void AzurirajKorisnika(Korisnik korisnik,Boolean tajmerpromjena=false)
        {
            var korisnici = getKorisnici();
            var starikorisnik= korisnici.FirstOrDefault(u => u.korisnickoIme == korisnik.korisnickoIme);
            if (starikorisnik != null)
            {
                starikorisnik.listaPodsjetnika = korisnik.listaPodsjetnika;
                starikorisnik.toDoLista = korisnik.toDoLista;
                
                SpremiKorisnike(korisnici);
                if(!tajmerpromjena) Console.WriteLine("Ažurirani podaci za korisnika: "+starikorisnik.korisnickoIme);
            }else
            {
                Console.WriteLine("Ne postoji taj korisnik");
            }
        }

        public void VecPostojiKorisnik(String korisnickoImetemp)
        {
            var korisnici = getKorisnici();
            foreach(var korisnik in korisnici)
            {
                if (korisnik.korisnickoIme == korisnickoImetemp)
                    throw new ArgumentException("Vec psotoji korisnik s tim korisnickim imenom!");

            }
        }
    }
}

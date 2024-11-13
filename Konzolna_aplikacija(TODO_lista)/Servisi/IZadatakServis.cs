using Konzolna_aplikacija_TODO_lista_.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konzolna_aplikacija_TODO_lista_.Servisi
{
    public interface IZadatakServis
    {
        void UnesiZadatak(Korisnik korisnik, Zadatak zadatak);
        void FiltrirajZadatke(List<Zadatak> zadaci, int opcija);
        void StatistikaListe(List<Zadatak> zadaci);
        void IzlistajSvePodsjetnike(Korisnik korisnik);
    }
}

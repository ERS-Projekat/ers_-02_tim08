using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DBServisi
{
    public class KorisniciRepozitorijum : IKorisniciRepozitorijum
    {
        public Korisnik DodajKorisnika(Korisnik korisnik)
        {
            try
            {
                korisnik.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + IBazaPodataka.Tabele.Korisnici.Count;



                IBazaPodataka.Tabele.Korisnici.Add(korisnik);
                IBazaPodataka.SacuvajPromene();


                return korisnik;
            }
            catch
            {
                return new Korisnik();
            }
        }
        public Korisnik PronadjiKorisnikaPoKorisnickomImenu(string korisnickoIme)
        {
            try
            {
                return IBazaPodataka.Tabele.Korisnici.FirstOrDefault(k => k.KorisnickoIme == korisnickoIme);
            }
            catch
            {
                return new Korisnik();
            }
        }
        public IEnumerable<Korisnik> SviKorisnici()
        {
            try
            {
                return IBazaPodataka.Tabele.Korisnici;
            }
            catch
            {
                return new List<Korisnik>();
            }
        }
    }
}

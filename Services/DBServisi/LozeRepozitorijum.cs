using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;

namespace Services.DBServisi
{
    public class LozeRepozitorijum : ILozeRepozitorijum
    {
        public Loza DodajLozu(Loza loza)
        {
            try {
                loza.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + IBazaPodataka.Tabele.Loza.Count;



                IBazaPodataka.Tabele.Loza.Add(loza);
                IBazaPodataka.SacuvajPromene();


                return loza;
            }
            catch 
            {
                return new Loza();
            }

        }
        public Loza PronadjiLozuPoId(long id)
        {
            try
            {
                return IBazaPodataka.Tabele.Loza.FirstOrDefault(l => l.Id == id);
            }
            catch
            {
                return new Loza();
            }
        }
        public IEnumerable<Loza> SveLoze()
        {
            try
            {
                return IBazaPodataka.Tabele.Loza;
            }
            catch 
            { 
                return new List<Loza>(); 
            }

        }
        public IEnumerable<Loza> PronadjiLozePoNazivu(string naziv)
        {
            try
            {
                return IBazaPodataka.Tabele.Loza.Where(l => l.Naziv== naziv);
            }
            catch
            {
                return new List<Loza>();
            }
        }
        public IEnumerable<Loza> PronadjiLozePoFaziZrelosti(FazaZrelosti faza)
        {
            try
            {
                return IBazaPodataka.Tabele.Loza.Where(l => l.FazaZrelosti == faza);
            }
            catch
            {
                return new List<Loza>();
            }
        }
        public bool AzurirajLozu(Loza loza)
        {
            try 
            {
                var PostojecaLoza = IBazaPodataka.Tabele.Loza.FirstOrDefault(l => l.Id == loza.Id);
                if (PostojecaLoza != null)
                {
                    int index = IBazaPodataka.Tabele.Loza.IndexOf(PostojecaLoza);



                    IBazaPodataka.Tabele.Loza[index] = loza;
                    IBazaPodataka.SacuvajPromene();


                    return true;
                }

                return false;

            }
            catch 
            { 
                return false; 
            }

        }
        public bool ObrisiLozu(long id)
        {
            try
            {
                var PostojecaLoza = IBazaPodataka.Tabele.Loza.FirstOrDefault(l => l.Id == id);
                if (PostojecaLoza != null)
                {
                    IBazaPodataka.Tabele.Loza.Remove(PostojecaLoza);
                    IBazaPodataka.SacuvajPromene();

                    return true;
                }

                return false;

            }
            catch
            {
                return false;
            }
        }
    }
}

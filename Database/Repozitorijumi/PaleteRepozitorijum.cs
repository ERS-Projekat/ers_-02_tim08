using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class PaleteRepozitorijum : IPaleteRepozitorijum
    {
        private readonly IBazaPodataka bazaPodataka;

        public PaleteRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public Paleta DodajPaletu(Paleta paleta)
        {
            try
            {
                paleta.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Palete.Count;
                paleta.Sifra = $"PAL-{DateTime.Now.Year}-{paleta.Id}";
                bazaPodataka.Tabele.Palete.Add(paleta);
                bazaPodataka.SacuvajPromene();
                return paleta;
            }
            catch
            {
                return new Paleta();
            }
        }

        public Paleta PronadjiPaletuPoId(long id)
        {
            try
            {
                return bazaPodataka.Tabele.Palete.FirstOrDefault(p => p.Id == id) ?? new Paleta();
            }
            catch
            {
                return new Paleta();
            }
        }

        public IEnumerable<Paleta> SvePalete()
        {
            try
            {
                return bazaPodataka.Tabele.Palete;
            }
            catch
            {
                return new List<Paleta>();
            }
        }

        public IEnumerable<Paleta> PronadjiPaletePoStatusu(StatusPalete status)
        {
            try
            {
                return bazaPodataka.Tabele.Palete.Where(p => p.Status == status);
            }
            catch
            {
                return new List<Paleta>();
            }
        }

        public bool AzurirajPaletu(Paleta paleta)
        {
            try
            {
                var postojecaPaleta = bazaPodataka.Tabele.Palete.FirstOrDefault(p => p.Id == paleta.Id);
                if (postojecaPaleta != null)
                {
                    int index = bazaPodataka.Tabele.Palete.IndexOf(postojecaPaleta);
                    bazaPodataka.Tabele.Palete[index] = paleta;
                    bazaPodataka.SacuvajPromene();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ObrisiPaletu(long id)
        {
            try
            {
                var paleta = bazaPodataka.Tabele.Palete.FirstOrDefault(p => p.Id == id);
                if (paleta != null)
                {
                    bazaPodataka.Tabele.Palete.Remove(paleta);
                    bazaPodataka.SacuvajPromene();
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
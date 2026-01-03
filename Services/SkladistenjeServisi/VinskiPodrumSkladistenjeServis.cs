using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;

namespace Services.SkladistenjeServisi
{
    public class VinskiPodrumSkladistenjeServis : ISkladistenjeServis
    {
        private readonly IPaleteRepozitorijum paleteRepozitorijum;
        public VinskiPodrumSkladistenjeServis(IPaleteRepozitorijum paleteRepozitorijum) 
        {
            this.paleteRepozitorijum = paleteRepozitorijum;
        }

        public List<Paleta> IsporuciPalete(int brojPaleta)
        {
            try
            {
                //Broj paleta ispravan
                if (brojPaleta < 0 || brojPaleta > 5)
                {
                return new List<Paleta>();
                }

                for(int i = 0; i < brojPaleta; i--)
                {
                    Thread.Sleep(300);
                }

                return isporucenePalete;
            }
            catch
            {
                return new List<Paleta>();
            }
        }
    }
}

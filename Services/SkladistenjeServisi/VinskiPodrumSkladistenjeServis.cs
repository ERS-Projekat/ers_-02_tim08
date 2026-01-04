using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.LoggerServisi;

namespace Services.SkladistenjeServisi
{
    public class VinskiPodrumSkladistenjeServis : ISkladistenjeServis
    {
        private readonly IPaleteRepozitorijum paleteRepozitorijum;
        private readonly ILoggerServis loggerServis;
        public VinskiPodrumSkladistenjeServis(IPaleteRepozitorijum paleteRepozitorijum, ILoggerServis loggerServis)
        {
            this.paleteRepozitorijum = paleteRepozitorijum;
            this.loggerServis = loggerServis;
        }

        public List<Paleta> IsporuciPalete(int brojPaleta)
        {
            try
            {
                int maxPaletaPoIsporuci = 5;
                if (brojPaleta > maxPaletaPoIsporuci)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Vinski podrum može isporučiti maksimalno {maxPaletaPoIsporuci} paleta po isporuci");
                    brojPaleta = maxPaletaPoIsporuci;
                }

                var dostupnePalete = paleteRepozitorijum.PronadjiPaletePoStatusu(StatusPalete.Otpremljena).Take(brojPaleta).ToList();

                if (dostupnePalete.Count == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Nema otpremljenih paleta");
                    return new List<Paleta>();
                }

                foreach (var paleta in dostupnePalete)
                {
                    Thread.Sleep(300);
                }
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Palete su isporucene");
                return dostupnePalete;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Desio se exception tokom isporuke paleta");
                return new List<Paleta>();
            }
        }
    }
}

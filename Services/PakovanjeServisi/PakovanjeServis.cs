using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services;
using Services.LoggerServisi;

namespace Services.PakovanjeServisi
{
    public class PakovanjeServis : IPakovanjeServis
    {
        private readonly IVinaRepozitorijum vinaRepozitorijum;
        private readonly IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum;
        private readonly IPaleteRepozitorijum paleteRepozitorijum;
        private readonly ILoggerServis loggerServis;
        public PakovanjeServis(IVinaRepozitorijum vinaRepozitorijum, IPaleteRepozitorijum paleteRepozitorijum, ILoggerServis loggerServis, IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum)
        {
            this.vinaRepozitorijum = vinaRepozitorijum;
            this.paleteRepozitorijum = paleteRepozitorijum;
            this.loggerServis = loggerServis;
            this.vinskiPodrumiRepozitorijum = vinskiPodrumiRepozitorijum;
        }
        public Paleta PakujVino(long idVinskogPodruma, string adresaOdredista, List<long> idVina)
        {
            try
            {
               
                foreach (var id in idVina)
                {
                    var vino = vinaRepozitorijum.PronadjiVinoPoId(id);
                    if (vino.Id == 0)
                    {
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Nismo nasli vino sa id: {id}!");
                        return new Paleta();
                    }
                }

                var svePalete = paleteRepozitorijum.SvePalete();

                foreach (var idVina_ in idVina)
                {
                    bool postojiNaPaleti = svePalete.Any(p => p.IdVina.Contains(idVina_));

                    if (postojiNaPaleti)
                    {
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Vino {idVina_} je već na paleti - ne može se dodati ponovo");
                        return new Paleta();
                    }
                }

                
                Paleta paleta = new Paleta();
                paleta.IdVina = idVina;
                paleta.AdresaOdredista = adresaOdredista;
                paleta.IdVinskogPodruma = idVinskogPodruma;

                paleta = paleteRepozitorijum.DodajPaletu(paleta);

                
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Vina su upakovana u paletu {paleta.Id}.");

                return paleta;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Pakovanje vina u paletu neuspesno!");
                return new Paleta();
            }
        }

        public bool PosaljiPaletuUPodrum(long idPalete)
        {
            try
            {
                
                var paleta = paleteRepozitorijum.PronadjiPaletuPoId(idPalete);
                if (paleta.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Slanje palete {idPalete} neuspesno!");
                    return false;
                }

                
                if (paleta.Status == StatusPalete.Upakovana)
                {
                    VinskiPodrum vinskiPodrum = vinskiPodrumiRepozitorijum.PronadjiVinskiPodrumPoId(paleta.IdVinskogPodruma);
                    if (vinskiPodrum.Id == 0 || vinskiPodrum.MaxBrojPaleta <= vinskiPodrum.IdPaleta.Count)
                    {
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Slanje palete {idPalete} neuspesno!");
                        return false;
                    }
                    vinskiPodrum.IdPaleta.Add(paleta.Id);
                    vinskiPodrumiRepozitorijum.AzurirajVinskiPodrum(vinskiPodrum);


                    paleta.Status = StatusPalete.Otpremljena;
                    paleteRepozitorijum.AzurirajPaletu(paleta);

                    
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Paleta {idPalete} poslata u podrum {paleta.IdVinskogPodruma}.");

                    return true;
                }
                else
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Slanje palete {idPalete} neuspesno!");
                    return false;
                }
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Slanje palete {idPalete} neuspesno!");
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Services;

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
                //Proverava da li vina postoje
                foreach (var id in idVina)
                {
                    var vino = vinaRepozitorijum.PronadjiVinoPoId(id);
                    if (vino.Id == 0)
                    {
                        return new Paleta();
                    }
                }

                //Kreira novu paletu
                Paleta paleta = new Paleta();
                paleta.IdVina = idVina;
                paleta.AdresaOdredista = adresaOdredista;
                paleta.IdVinskogPodruma = idVinskogPodruma;

                paleta = paleteRepozitorijum.DodajPaletu(paleta);

                //Logovanje
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Pakovanje vina u paletu {paleta.Id}.");

                return paleta;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Pakovanje vina u paletu neuspesno!");
                return new Paleta();
            }
        }

        public bool PosaljiPaletuUPodrum(long idPalete)
        {
            try
            {
                //Pronalazi paletu
                var paleta = paleteRepozitorijum.PronadjiPaletuPoId(idPalete);
                if (paleta.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Slanje palete {idPalete} neuspesno!");
                    return false;
                }

                //salje paletu u vinski podrum
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

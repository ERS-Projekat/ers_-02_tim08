using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ProizvodnjaVinaServisi
{
    public class ProizvodnjaVinaServis : IProizvodnjaVinaServis
    {
        private readonly IVinaRepozitorijum vinaRepozitorijum;
        private readonly ILozeRepozitorijum lozeRepozitorijum;
        private readonly IVinogradurstvoServis vinogradurstvoServis;
        private readonly ILoggerServis loggerServis;


        public ProizvodnjaVinaServis(
            IVinaRepozitorijum vinaRepozitorijum,
            ILozeRepozitorijum lozeRepozitorijum,
            IVinogradurstvoServis vinogradurstvoServis,
            ILoggerServis loggerServis)
        {
            this.vinaRepozitorijum = vinaRepozitorijum;
            this.lozeRepozitorijum = lozeRepozitorijum;
            this.vinogradurstvoServis = vinogradurstvoServis;
            this.loggerServis = loggerServis;
        }

        public List<Vino> ZapocniFermentaciju(string nazivVina, KategorijaVina kategorija, int brojFlasa, double zapreminaFlase)
        {
            try
            {
                double potrebnaKolicinaVina = brojFlasa * zapreminaFlase;
                int potrebnoBrojLoza = (int)Math.Ceiling(potrebnaKolicinaVina / 1.2);

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                    $"Započeta fermentacija: potrebno {potrebnoBrojLoza} loza za {brojFlasa} flaša od {zapreminaFlase}L");

                var obraneLoze = lozeRepozitorijum.PronadjiLozePoFaziZrelosti(FazaZrelosti.Obrana)
                    .Take(potrebnoBrojLoza)
                    .ToList();

                if (obraneLoze.Count < potrebnoBrojLoza)
                {
                    int nedostaje = potrebnoBrojLoza - obraneLoze.Count;
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING,
                        $"Nedostaje {nedostaje} loza za fermentaciju");

                    for (int i = 0; i< nedostaje; i++)
                    {
                        Loza novaLoza = vinogradurstvoServis.PosadiNovuLozu(nazivVina, "Toskana");
                        novaLoza.FazaZrelosti = FazaZrelosti.SpremnaZaBerbu;
                        lozeRepozitorijum.AzurirajLozu(novaLoza);

                        var obereneLoze = vinogradurstvoServis.OberiLoze(nazivVina, 1);
                        if (obereneLoze.Count > 0)
                            obraneLoze.Add(obereneLoze[0]);
                    }
                }

                double optimalniBrix = 24.0;
                foreach (var loza in obraneLoze)
                {
                    if (loza.NivoSecera > optimalniBrix)
                    {
                        double razlika = loza.NivoSecera - optimalniBrix;
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                            $"Loza ID {loza.Id} ima previsok nivo šećera ({loza.NivoSecera} Brix), potrebno balansiranje");

                        Loza balansirajucaLoza = vinogradurstvoServis.PosadiNovuLozu(nazivVina, "Toskana");
                        double noviNivo = balansirajucaLoza.NivoSecera - razlika;
                        balansirajucaLoza.NivoSecera = Math.Max(15.0, noviNivo);

                        lozeRepozitorijum.AzurirajLozu(balansirajucaLoza);

                        loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                            $"Posađena balansirajuća loza sa nivoom šećera {balansirajucaLoza.NivoSecera} Brix");
                    }
                }

                List<Vino> proizvedenaVina = new List<Vino>();
                for(int i = 0; i < brojFlasa; i++)
                {
                    Vino vino = new Vino(nazivVina, kategorija, zapreminaFlase, obraneLoze[i % obraneLoze.Count].Id);
                    vino = vinaRepozitorijum.DodajVino(vino);
                    proizvedenaVina.Add(vino);
                }
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                    $"Fermentacija završena: proizvedeno {proizvedenaVina.Count} flaša vina '{nazivVina}'");

                return proizvedenaVina;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR,
                    $"Greška pri fermentaciji: {ex.Message}");
                return new List<Vino>();
            }
        }

        public List<Vino> DobijProizvedenaVina(KategorijaVina kategorija, int kolicina)
        {
            try
            {
                var vina = vinaRepozitorijum.PronadjiVinaPoKategoriji(kategorija)
                    .Take(kolicina)
                    .ToList();

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                    $"Vraćeno {vina.Count} vina kategorije {kategorija}");

                return vina;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR,
                    $"Greška pri dobijanju vina: {ex.Message}");
                return new List<Vino>();
            }
        }
    }
}
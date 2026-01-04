using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;

namespace Services.VinogradarstvoServisi
{
    internal class VinogradarstvoServis : IVinogradurstvoServis
    {

        private readonly ILozeRepozitorijum lozeRepozitorijum;
        private readonly ILoggerServis loggerServis;


        public VinogradarstvoServis(ILozeRepozitorijum lozeRepozitorijum, ILoggerServis loggerServis)
        {
            this.lozeRepozitorijum = lozeRepozitorijum;
            this.loggerServis = loggerServis;
        }

        public Loza PosadiNovuLozu(string naziv, string regionUzgoja)
        {
            try
            {
                Random nivoSecera = new Random();

                Loza novaLoza = new Loza(naziv, nivoSecera.NextDouble() * (28.0 - 15.0) + 15.0, DateTime.UtcNow.Year, regionUzgoja);

                novaLoza.FazaZrelosti = FazaZrelosti.Posadjena;

                lozeRepozitorijum.DodajLozu(novaLoza);

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Uspesno evidentirano sađenje nove loze {naziv} u {regionUzgoja}");
                
                return novaLoza;    
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greška pri sadjenju: {ex.Message}");
                return  new Loza();
            }
        }


        public bool PromeniNivoSecera(long idLoze, double procenat)
        {
            try
            {
                if (procenat < -5 || procenat > 10) {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Procenat nije između -5 i +10 ({procenat})");
                    return false;
                }

                Loza temp = lozeRepozitorijum.PronadjiLozuPoId(idLoze);

                if (temp == null)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Loza sa ID {idLoze} nije pronađena!");
                    return false;
                }

                temp.NivoSecera += temp.NivoSecera/100 * procenat ;

                if (temp.NivoSecera < 15.0) temp.NivoSecera = 15.0;
                if (temp.NivoSecera > 28.0) temp.NivoSecera = 28.0;

                lozeRepozitorijum.AzurirajLozu(temp);
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,$"Nivo šećera loze {idLoze} promenjen na: {temp.NivoSecera}");

                return true;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greška pri promeni nivoa šećera: {ex.Message}");
                return false;
            }
        }
        public List<Loza> OberiLoze(string nazivSorte, int brojLoza)
        {
            try
            {
                List<Loza> lozeSaNazivom = (List<Loza>)lozeRepozitorijum.PronadjiLozePoNazivu(nazivSorte);
                List<Loza> obraneLoze = new List<Loza>();
                List<Loza> vraceneLoze = new List<Loza>();
                int brojLozaUBazi = 0;

                for (int i = 0; i < lozeSaNazivom.Count; i++)
                {
                    if (lozeSaNazivom[i].FazaZrelosti == FazaZrelosti.SpremnaZaBerbu)
                    {
                        brojLozaUBazi++;
                        obraneLoze.Add(lozeSaNazivom[i]);
                    }
                }

                if (brojLoza < brojLozaUBazi)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"U bazi ima manje od {brojLoza} loza sorte {nazivSorte} spremnih za branje ({brojLozaUBazi})");
                    return new List<Loza>();
                }
                for (int i = 0; i < brojLoza; i++)
                {
                    obraneLoze[i].FazaZrelosti = FazaZrelosti.Obrana;
                    vraceneLoze.Add(obraneLoze[i]);
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Obrano {brojLoza} zrelih loza sorte {nazivSorte}");
                return vraceneLoze;
            }
            catch (Exception ex) 
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Neuspešno branje:{ex.Message}");
                return new List<Loza>();
            }
        }

    }
}

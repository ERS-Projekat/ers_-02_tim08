using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Modeli.Enumeracije;


namespace Database.Repozitorijumi
{
    public class VinaRepozitorijum : IVinaRepozitorijum

    {

        private readonly IBazaPodataka bazaPodataka;

        public VinaRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public Vino DodajVino(Vino vino) 
        {
            try
            {
                vino.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Vina.Count;
                bazaPodataka.Tabele.Vina.Add(vino);
                bazaPodataka.SacuvajPromene();
                return vino;
            }
            catch
            {
                return new Vino();
            }

        }
        public Vino PronadjiVinoPoId(long id) 
        {
            try
            {
                return bazaPodataka.Tabele.Vina.FirstOrDefault(v => v.Id == id) ?? new Vino();
            }
            catch
            {
                return new Vino();
            }
        }
        public IEnumerable<Vino> SvaVina() 
        {
            try
            {
                return bazaPodataka.Tabele.Vina;
            }
            catch
            {
                return new List<Vino>();
            }
        }
        public IEnumerable<Vino> PronadjiVinaPoKategoriji(KategorijaVina kategorija) 
        {
            try
            {
                return bazaPodataka.Tabele.Vina.Where(v => v.Kategorija == kategorija).ToList() ?? new List<Vino>();
            }
            catch
            {
                return new List<Vino>();
            }
        }
        public bool AzuzirajVino(Vino vino) 
        {
            try
            {
                var postojeceVino = bazaPodataka.Tabele.Vina.FirstOrDefault(v => v.Id == vino.Id);
                if (postojeceVino != null)
                {
                    int index = bazaPodataka.Tabele.Vina.IndexOf(postojeceVino);
                    bazaPodataka.Tabele.Vina[index] = vino;
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
        bool ObrasniVino(long id);

    }
}

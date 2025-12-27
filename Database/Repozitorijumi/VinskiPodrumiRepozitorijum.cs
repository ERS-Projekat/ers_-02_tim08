using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;


namespace Database.Repozitorijumi
{
    public class VinskiPodrumiRepozitorijum : IVinskiPodrumiRepozitorijum
    {
        private readonly IBazaPodataka bazaPodataka;

        public VinskiPodrumiRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public VinskiPodrum DodajVinskiPodrum(VinskiPodrum podrum)
        {
            try
            {
                podrum.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.VinskiPodrumi.Count;
                bazaPodataka.Tabele.VinskiPodrumi.Add(podrum);
                bazaPodataka.SacuvajPromene();
                return podrum;
            }
            catch
            {
                return new VinskiPodrum();
            }
        }

        public VinskiPodrum PronadjiVinskiPodrumPoId(long id)
        {
            try
            {
                return bazaPodataka.Tabele.VinskiPodrumi.FirstOrDefault(vp => vp.Id == id) ?? new VinskiPodrum();
            }
            catch
            {
                return new VinskiPodrum();
            }
        }

        public IEnumerable<VinskiPodrum> SviVinskiPodrumi()
        {
            try
            {
                return bazaPodataka.Tabele.VinskiPodrumi;
            }
            catch
            {
                return new List<VinskiPodrum>();
            }
        }

        public bool AzurirajVinskiPodrum(VinskiPodrum podrum)
        {
            try
            {
                var postojeciPodrum = bazaPodataka.Tabele.VinskiPodrumi.FirstOrDefault(vp => vp.Id == podrum.Id);
                if (postojeciPodrum != null)
                {
                    int index = bazaPodataka.Tabele.VinskiPodrumi.IndexOf(postojeciPodrum);
                    bazaPodataka.Tabele.VinskiPodrumi[index] = podrum;
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

        public bool ObrisiVinskiPodrum(long id)
        {
            try
            {
                var podrum = bazaPodataka.Tabele.VinskiPodrumi.FirstOrDefault(vp => vp.Id == id);
                if (podrum != null)
                {
                    bazaPodataka.Tabele.VinskiPodrumi.Remove(podrum);
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

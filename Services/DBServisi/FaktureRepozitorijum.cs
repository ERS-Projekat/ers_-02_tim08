using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DBServisi
{
    public class FaktureRepozitorijum : IFaktureRepozitorijum
    {
        public Faktura DodajFakturu(Faktura faktura)
        {
            try
            {
                faktura.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + IBazaPodataka.Tabele.Faktura.Count;



                IBazaPodataka.Tabele.Faktura.Add(faktura);
                IBazaPodataka.SacuvajPromene();


                return faktura;
            }
            catch
            {

                return new Faktura();

            }
        }
        public Faktura PronadjiFakturuPoId(long id)
        {
            try
            {
                return IBazaPodataka.Tabele.Faktura.FirstOrDefault(f => f.Id == id);
            }
            catch
            {
                return new Faktura();
            }
        }
        public IEnumerable<Faktura> SveFakture()
        {
            try
            {
                return IBazaPodataka.Tabele.Faktura;
            }
            catch
            {
                return new List<Faktura>();
            }
        }
        public bool AzurirajFakturu(Faktura faktura)
        {
            try
            {
                var PostojecaFaktura = IBazaPodataka.Tabele.Faktura.FirstOrDefault(f => f.Id == faktura.Id);
                if (PostojecaFaktura != null)
                {
                    int index = IBazaPodataka.Tabele.Faktura.IndexOf(PostojecaFaktura);



                    IBazaPodataka.Tabele.Faktura[index] = faktura;
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

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthServer.Service
{
    public class ObjectMapper
    {
        //Ben datayı alama kadar memory de bu static arkadaş bulunmasın.  LazyLoading : Sadece ihtiyaç oldugu anda eklesin. Eğer bunu kullanmazsak ilk uygulama ayağa
        //kalktığı anda ObjectMapper içerisindeki data memorye hemen yüklenir.  Biz istiyoruz ki ObjectMapper içerisindeki arakdaşlar ben istediğim zaman yüklensin daha sonra memoryden çekeriz.
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoMapper>();
            });
            return config.CreateMapper();
        });
        public static IMapper Mapper => lazy.Value;
    }
}

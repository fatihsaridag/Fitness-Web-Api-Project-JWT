using FitnessAuthSever.Core.Dtos;
using FitnessAuthSever.Core.Entity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthSever.Core.Services
{
    public interface ITokenService
    {
        TokenDto CreateToken(UserApp userApp);  //Bir kullanıcı alıp Bir token oluşturacak.
    }
}

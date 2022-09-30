using FitnessAuthSever.Core.Dtos;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthSever.Core.Services
{
    public interface IUserService
    {
        Task<Response<UserAppDto>> CreateAsync(CreateUserDto createUserDto);
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);         //Clientten bir kullanıcı adı alarak Kullanıcıyı getirmemizi sağlayacak.
    }
}

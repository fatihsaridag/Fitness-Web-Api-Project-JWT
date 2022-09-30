using FitnessAuthSever.Core.Dtos;
using FitnessAuthSever.Core.Entity;
using FitnessAuthSever.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager;
        }


        public async Task<Response<UserAppDto>> CreateAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp { Email = createUserDto.Email, UserName = createUserDto.UserName };
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
            }
            //Eğer işlem başarılı ise Mapleme işlemi gerçekleştirerek user UserAppDto ya mapllenir.
            var newUserDto = ObjectMapper.Mapper.Map<UserAppDto>(user);
            return Response<UserAppDto>.Success(newUserDto, 200);
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Response<UserAppDto>.Fail("User is not found", 404,true);
            }
            var newUserDto = ObjectMapper.Mapper.Map<UserAppDto>(user);
            return Response<UserAppDto>.Success(newUserDto, 200);
        }
    }
}

using FitnessAuthSever.Core.Dtos;
using FitnessAuthSever.Core.Entity;
using FitnessAuthSever.Core.Repositories;
using FitnessAuthSever.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;


        public AuthenticationService(UserManager<UserApp> userManager, ITokenService tokenService, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        //Kullanıcı olan token için token oluşturma işlemi.
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 404, true);
            }
            if (!await _userManager.CheckPasswordAsync(user,loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 404, true);
            }
            //Artık bu satırda kullanıcı olduguna eminiz.
            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            //Eğer RefreshToken yok ise yeni bir refreshToken oluşturuyoruz.
            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;

            }

            await _unitOfWork.SaveChangesAsync();
            return Response<TokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var isExistRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (isExistRefreshToken == null)
            {
                return Response<TokenDto>.Fail("Refresh Token Not found ", 404, true);
            }
            //Eğerki refreshToken var ise userId de vardır. 
            var user = await _userManager.FindByIdAsync(isExistRefreshToken.UserId);
            if (user == null)
            {
                return Response<TokenDto>.Fail("Refresh Token not found", 404, true);
            }
            //Eğer user var ise 
            var tokenDto = _tokenService.CreateToken(user);
            isExistRefreshToken.Code = tokenDto.RefreshToken;
            isExistRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            await _unitOfWork.SaveChangesAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken =  await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh Token Not found", 404, true);
            }

            //Eğer refresh Tokenımız var ise 

            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            return Response<NoDataDto>.Success(200);


        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomTokenAuth
    {
        public static void AddCustomTokenAuth(this IServiceCollection services, CustomTokenOptions tokenOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;         //Bu Authentication daki şema 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;            //Bu da ikisinin iletişime geçmesi için

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt => {                         //Bu da JwtBearer dan gelen şema

                //CustomTokenOptions dan bir nesne örneği allmmamız lazım o yüzden 
                //Validation parametrelerini belirleyeceğimiz için 
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero


                };
            });
        }
    }
}

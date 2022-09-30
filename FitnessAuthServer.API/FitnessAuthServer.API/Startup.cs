using FitnessAuthServer.Data;
using FitnessAuthServer.Data.Repositories;
using FitnessAuthServer.Service.Services;
using FitnessAuthSever.Core.Entity;
using FitnessAuthSever.Core.Repositories;
using FitnessAuthSever.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessAuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Artýk IAuthenticationService ile karþýlaþtýðýnda AuthenticationService den nesne örneði alacaðýnýz biliyoruz.
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IGenericService<,>), typeof(GenericServices<,>)); //Ýki tane generic entity aldýðýndan virgül koyuyoruz. Eðerki 3 tane generic alsaydý 2 tane virgül koyuyoruz.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"),sqlOptions => {

                    sqlOptions.MigrationsAssembly("FitnessAuthServer.Data");
                });

            });


            //Benim kullanýcým UserApp, Rolüm ise IdentityRole
            services.AddIdentity<UserApp, IdentityRole>(Opt =>
            {
                Opt.User.RequireUniqueEmail = true;             //Emailim veritabanýnda uniq olsun.
                Opt.Password.RequireNonAlphanumeric = false;    // passwordda alfa numeric karakteri false yaptýk.(A-Z) Non alfa numeric zorunlu deðil.
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //Hangi DbContext . Þifre sýfýrlamalarda token üretmek için.


            //TOKEN DOÐRULAMA ÝÞLEMLERÝ 


            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;         //Bu Authentication daki þema 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;            //Bu da ikisinin iletiþime geçmesi için

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opt => {                         //Bu da JwtBearer dan gelen þema

                //CustomTokenOptions dan bir nesne örneði allmmamýz lazým o yüzden 
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();
                //Validation parametrelerini belirleyeceðimiz için 
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer  = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero


                };
            });


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FitnessAuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FitnessAuthServer.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

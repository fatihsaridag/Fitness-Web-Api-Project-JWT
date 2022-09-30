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
            //Art�k IAuthenticationService ile kar��la�t���nda AuthenticationService den nesne �rne�i alaca��n�z biliyoruz.
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IGenericService<,>), typeof(GenericServices<,>)); //�ki tane generic entity ald���ndan virg�l koyuyoruz. E�erki 3 tane generic alsayd� 2 tane virg�l koyuyoruz.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"),sqlOptions => {

                    sqlOptions.MigrationsAssembly("FitnessAuthServer.Data");
                });

            });


            //Benim kullan�c�m UserApp, Rol�m ise IdentityRole
            services.AddIdentity<UserApp, IdentityRole>(Opt =>
            {
                Opt.User.RequireUniqueEmail = true;             //Emailim veritaban�nda uniq olsun.
                Opt.Password.RequireNonAlphanumeric = false;    // passwordda alfa numeric karakteri false yapt�k.(A-Z) Non alfa numeric zorunlu de�il.
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //Hangi DbContext . �ifre s�f�rlamalarda token �retmek i�in.


            //TOKEN DO�RULAMA ��LEMLER� 


            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;         //Bu Authentication daki �ema 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;            //Bu da ikisinin ileti�ime ge�mesi i�in

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opt => {                         //Bu da JwtBearer dan gelen �ema

                //CustomTokenOptions dan bir nesne �rne�i allmmam�z laz�m o y�zden 
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();
                //Validation parametrelerini belirleyece�imiz i�in 
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

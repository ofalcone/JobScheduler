using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace JobScheduler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllersWithViews();

            services.AddDbContext<JobSchedulerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            //services.AddScoped<DbContextUtility>();
            //services.AddScoped<UserUtility>();


            //Ogni utente può avere N ruoli, il ruolo è una stringa
            //config server per impostare dei parametri di autenticazione
            services.AddIdentity<User, IdentityRole>(config =>
             { config.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<JobSchedulerContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                    .AddCookie()
                    .AddJwtBearer(config =>
                    {
                        config.TokenValidationParameters = new TokenValidationParameters
                        {
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                            //chi emette il jwt
                            ValidateIssuer = false,
                            //client che usano i jwt
                            ValidateAudience = false,

                            //tutti e due in false perche "ci serviamo" di jwt per l autenticazione
                        };
                    });

            services.AddScoped<JobSchedulerDataSeed>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //Middleware aspnetcore.Identity.EntityFramework che filtra le richieste in base all'autenticazione
            //Va messo prima di UseRouting e UseAuthorization
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });

            using var scope = app.ApplicationServices.CreateScope();
            var seed = scope.ServiceProvider.GetService<JobSchedulerDataSeed>();
            seed.SeedAsync().Wait();

            app.UseHttpContext();
        }
    }
}

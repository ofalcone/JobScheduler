using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Interfaces;
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

        //The ConfigureServices() method does not allow injecting services, it only accepts an IServiceCollection argument.
        //This makes sense because ConfigureServices() is where you register the services required by your application.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllersWithViews();

            services.AddDbContext<JobSchedulerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            //Ogni utente pu� avere N ruoli, il ruolo � una stringa
            //config server per impostare dei parametri di autenticazione
            services.AddIdentity<User, IdentityRole>(
                config => { config.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<JobSchedulerContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                    .AddCookie()
                    .AddJwtBearer(config =>
                    {
                        config.RequireHttpsMetadata = false;
                        config.SaveToken = true;
                        config.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            //Encoding.ASCII.GetBytes(appSettings.Secret);
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                            //chi emette il jwt
                            ValidateIssuer = false,
                            //client che usano i jwt
                            ValidateAudience = false,

                            //tutti e due in false perche "ci serviamo" di jwt per l autenticazione
                        };
                    });
            //IdentityModelEventSource.ShowPII = true;
            services.AddScoped<JobSchedulerDataSeed>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //esplode
            //services.AddScoped<ScheduleJob>(); 

            //services.AddSingleton<IScheduleJob, ScheduleJob>();

            //// Build the intermediate service provider
            //var sp = services.BuildServiceProvider();

            //// This will succeed.
            //var jobSchedulerService = sp.GetService<IScheduleJob>();
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

            //Middleware aspnetcore.Identity.EntityFramework che filtra le richieste in base all'autenticazione: va messo prima di UseRouting e UseAuthorization
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

            //app.ApplicationServices.GetService<ScheduleJob>();
        }
    }
}

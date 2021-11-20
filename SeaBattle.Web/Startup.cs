using System;
using System.IO;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SeaBattle.Web.Data;
using SeaBattle.Web.Handlers;
using SeaBattle.Web.Managers;
using SeaBattle.Web.Middlewares;
using SeaBattle.Web.Models;

namespace SeaBattle.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<GameStateHandler>();
            services.AddScoped<GameWebSocketHandler>();
            services.AddScoped<InfoWebSocketHandler>();
            services.AddScoped<GameConnectionManager>();
            services.AddScoped<InfoConnectionManager>();
            services.AddScoped<GameManager>();

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=Qwerty1234");
            });

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>();

            services.AddMvc();

            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWebSockets();

            app.Map("/ws", x =>
                x.UseMiddleware<WebSocketMiddleware>(serviceProvider.GetService<GameWebSocketHandler>() ??
                                                     throw new NullReferenceException()));

            app.Map("/info", x =>
                x.UseMiddleware<WebSocketMiddleware>(serviceProvider.GetService<InfoWebSocketHandler>() ??
                                                     throw new NullReferenceException()));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
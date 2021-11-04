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
            services.AddSingleton<GameStateHandler>();
            services.AddSingleton<GameWebSocketHandler>();
            services.AddSingleton<InfoWebSocketHandler>();
            services.AddSingleton<GameConnectionManager>();
            services.AddSingleton<InfoConnectionManager>();
            services.AddSingleton<GameManager>();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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
                x.UseMiddleware<WebSocketMiddleware>(serviceProvider.GetService<GameWebSocketHandler>()));
            
            app.Map("/info", x =>
                x.UseMiddleware<WebSocketMiddleware>(serviceProvider.GetService<InfoWebSocketHandler>()));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapGet("/game.html",
                    async context => { await context.Response.WriteAsync(await File.ReadAllTextAsync("game.html")); });
            });
        }
    }
}
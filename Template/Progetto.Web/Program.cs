using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.Infrastructure;
using Template.Services;

namespace Progetto.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<TemplateDbContext>();
                TemplateDbContextSeed.Seed(db);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(kestrel =>
                    {
                        kestrel.AddServerHeader = false; // OWASP: Remove Kestrel response header 
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
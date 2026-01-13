using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
//using Progetto.Web.Hubs;
using Progetto.Web.Infrastructure;
using Progetto.Web.SignalR.Hubs;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Template.Infrastructure;
using Template.Services;

namespace Progetto.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Env = env;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddDbContext<TemplateDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "Template");
            });

            // SERVICES FOR AUTHENTICATION
            services.AddSession(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Login/Login";
                options.LogoutPath = "/Login/Logout";

                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                options.Cookie.Name = ".AspNetCore.Cookies";
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            var builder = services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {                        // Enable loading SharedResource for ModelLocalizer
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                });

#if DEBUG
            builder.AddRazorRuntimeCompilation();
#endif

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Areas/{2}/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

                options.ViewLocationFormats.Clear();
                options.ViewLocationFormats.Add("/Features/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Features/Views/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Features/Views/Shared/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            // SIGNALR FOR COLLABORATIVE PAGES
            services.AddSignalR();

            // CONTAINER FOR ALL EXTRA CUSTOM SERVICES
            Container.RegisterTypes(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApp Rendicontazione Api",
                    Version = "v1"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                // Https redirection only in production
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Localization support if you want to
            app.UseRequestLocalization(SupportedCultures.CultureNames);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp Rendicontazione Api v1");
            });

            app.UseRouting();

            // Adding authentication to pipeline
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            var node_modules = new CompositePhysicalFileProvider(Directory.GetCurrentDirectory(), "node_modules");
            var areas = new CompositePhysicalFileProvider(Directory.GetCurrentDirectory(), "Areas");
            var compositeFp = new CustomCompositeFileProvider(env.WebRootFileProvider, node_modules, areas);
            env.WebRootFileProvider = compositeFp;
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                // ROUTING PER HUB
                endpoints.MapHub<TemplateHub>("/templateHub");

                endpoints.MapAreaControllerRoute("TimeTracking", "TimeTracking", "TimeTracking/{controller=TimeTracking}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute("Administration", "Administration", "Administration/{controller=Admin}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute("Projects", "Projects", "Projects/{controller=Projects}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute("Example", "Example", "Example/{controller=Users}/{action=Index}/{id?}");    
                endpoints.MapControllerRoute("default", "{controller=Login}/{action=Login}");
            });
        }
    }

    public static class SupportedCultures
    {
        public readonly static string[] CultureNames;
        public readonly static CultureInfo[] Cultures;

        static SupportedCultures()
        {
            CultureNames = new[] { "it-it" };
            Cultures = CultureNames.Select(c => new CultureInfo(c)).ToArray();

            //NB: attenzione nel progetto a settare correttamente <NeutralLanguage>it-IT</NeutralLanguage>
        }
    }
}

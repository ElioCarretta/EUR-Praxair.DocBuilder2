using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Praxair.Web.Base.Localization.Json;

namespace Praxair.Web.DocBuilder2
{
    public class Startup
    {
        public readonly IFileProvider _fileProvider;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {

                _fileProvider = new CompositeFileProvider(
                    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                    new PhysicalFileProvider(Path.GetFullPath(@"..\Praxair.Web.Base\wwwroot")),
                    new PhysicalFileProvider(Path.GetFullPath(@"..\Praxair.Web.Base\Razor")),
                    new PhysicalFileProvider(Path.GetFullPath(@"..\Praxair.Web.Static\wwwroot")),
                    new PhysicalFileProvider(Path.GetFullPath(@"..\Praxair.Web.Components\wwwroot")),
                    new PhysicalFileProvider(Path.GetFullPath(@"..\Praxair.Web.Components\Razor"))
                    );
            }
            else
            {
                _fileProvider = new CompositeFileProvider(
                    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")) as IFileProvider,
                    new EmbeddedFileProvider(typeof(Static.Handle).GetTypeInfo().Assembly, @"Praxair.Web.Base.wwwroot") as IFileProvider,
                    new EmbeddedFileProvider(typeof(Static.Handle).GetTypeInfo().Assembly, @"Praxair.Web.Base.Razor") as IFileProvider,
                    new EmbeddedFileProvider(typeof(Static.Handle).GetTypeInfo().Assembly, @"Praxair.Web.Static.wwwroot") as IFileProvider,
                    new EmbeddedFileProvider(typeof(Components.Handle).GetTypeInfo().Assembly, @"Praxair.Web.Components.wwwroot") as IFileProvider,
                    new EmbeddedFileProvider(typeof(Components.Handle).GetTypeInfo().Assembly, @"Praxair.Web.Components.Razor") as IFileProvider
                    );
            }


            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_fileProvider);
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(_fileProvider);
                options.FileProviders.Add(new EmbeddedFileProvider(typeof(Part.Login.IdentityServer.Controllers.AccountController).GetTypeInfo().Assembly));
            });

            services.AddMemoryCache();
            services.AddJsonLocalization();


            var assembly = typeof(Part.Login.IdentityServer.Controllers.AccountController).GetTypeInfo().Assembly;

            services.AddMvc()
                .AddApplicationPart(assembly)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            services.AddDbContext<Part.Login.IdentityServer.Data.ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<Base.Model.Account.ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<Part.Login.IdentityServer.Data.ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddTransient<Part.Login.IdentityServer.Services.IEmailSender, Part.Login.IdentityServer.Services.EmailSender>();


            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.Expiration = TimeSpan.FromDays(150);
            //    options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
            //    options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
            //    options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
            //    options.SlidingExpiration = true;
            //});

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<Base.Interfaces.INavigationRepository, Demo.Repositories.NavigationRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseAuthentication();

            #region Localization

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("it-IT")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            #endregion
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = _fileProvider
            });
        }
    }
}

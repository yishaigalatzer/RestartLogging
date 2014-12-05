using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;
using RestartLogging.Logging;
using RestartLogging.Models;

namespace RestartLogging
{
    public class Startup
    {
        public static DateTime StartUpTime { get; set; }
        public Startup(IHostingEnvironment env)
        {
            StartUpTime = DateTime.Now;

            // Setup configuration sources.
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            //// Add EF services to the services container.
            //services.AddEntityFramework(Configuration)
            //    .AddSqlServer()
            //    .AddDbContext<ApplicationDbContext>();

            ////// Add Identity services to the services container.
            //services.AddDefaultIdentity<ApplicationDbContext, ApplicationUser, IdentityRole>(Configuration);

            // Add MVC services to the services container.
            services.AddMvc();

            // Uncomment the following line to add Web API servcies which makes it easier to port Web API 2 controllers.
            // You need to add Microsoft.AspNet.Mvc.WebApiCompatShim package to project.json
            // services.AddWebApiConventions();

        }

        public static DateTime ThisRequestStart { get; set; }
        public static ILogger TimingLogger { get; set; }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory, IApplicationEnvironment appEnv)
        {
            var folderPath = Path.Combine(
                Path.GetTempPath(),
                "ProjectKLogs");

            Directory.CreateDirectory(folderPath);

            loggerfactory.AddFile(Path.Combine(folderPath, appEnv.ApplicationName) + ".app.log");

            app.Use(async (ctx, next) =>
            {
                ThisRequestStart = DateTime.Now;
                await next();
            });

            // Configure the HTTP request pipeline.
            // Add the console logger. 
            TimingLogger = loggerfactory.Create("timing");

            using (var scope = TimingLogger.BeginScope("startup"))
            {
                // Add the following to the request pipeline only in development environment.
                if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
                {
                    //app.UseBrowserLink();
                    app.UseErrorPage(ErrorPageOptions.ShowAll);
                    //app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
                }
                else
                {
                    // Add Error handling middleware which catches all application specific errors and
                    // send the request to the following path or controller action.
                    app.UseErrorHandler("/Home/Error");
                }

                // Add static files to the request pipeline.
                app.UseStaticFiles();

                // Add cookie-based authentication to the request pipeline.
                //app.UseIdentity();

                // Add MVC to the request pipeline.
                app.UseMvc(routes =>
                {
                    routes.DefaultHandler = new LoggingRouter(routes.DefaultHandler, TimingLogger);

                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}/{id?}",
                        defaults: new { controller = "Home", action = "Index" });

                    // Uncomment the following line to add a route for porting Web API 2 controllers.
                    // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
                });
            }
        }
    }
}

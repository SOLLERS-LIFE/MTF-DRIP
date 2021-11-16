using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;

using MTFramework.Utilities;

using MTFDRIP.ApplicationDB.Data;

namespace MTFDRIP
{
    public class Startup
    {
        public Startup(IConfiguration configuration,
                       IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            GlobalParameters.Fulfill(Configuration, env);
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment _env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString_adb = Configuration.GetConnectionString("AppDBConnectionMySQL");
            services.AddDbContext<AppDB_Context>(
                options => options.UseMySql(connectionString_adb,
                                            ServerVersion.AutoDetect(connectionString_adb)
                                            ));

            services.AddControllersWithViews(config =>
            {
                config.RespectBrowserAcceptHeader = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BENEDICTUS",
                    Description = "Unified WEB API for products",
                    Contact = new OpenApiContact
                    {
                        Name = "Valerii Danilov",
                        Email = string.Empty,
                        Url = new Uri("https://www.linkedin.com/in/valerii-danilov-851505124/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }
                });
                c.EnableAnnotations();
                // Set the comments path for the Swagger JSON and UI.
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                                                  $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"),
                                     includeControllerXmlComments: true
                                    );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                              ILoggerFactory loggerFactory,
                              AppDB_Context adb)
        {
            migrater.Migrate(app);
            AppDBInitializer.DoIt(adb).Wait();

            GlobalParameters.setLoggerFactory(loggerFactory);

            if (GlobalParameters._isDevelopment)
            {
                app.UseExceptionHandler("/sysctl/error");
            }
            else
            {
                app.UseExceptionHandler("/sysctl/error");
            }

            // allow to know real ip if use revers proxy server
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BENEDICTUS v1");
                c.RoutePrefix = string.Empty; // ; "BNDv1"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

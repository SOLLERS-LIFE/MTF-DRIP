using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using MySqlConnector.Authentication.Ed25519;

using NLog;
using NLog.Web;

using MTFramework.Utilities;
using MTFDRIP.ApplicationDB.Data;

namespace MTFDRIP
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Trick to find if in migration routins
            // or any other external actions
            GlobalParameters.IsStartedWithMain = true;

            // to enable new user verification
            // with MySqlConnector.Authentication.Ed25519 NuGet Package
            Ed25519AuthenticationPlugin.Install();

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            NLog.GlobalDiagnosticsContext.Set("AppIdent", GlobalParameters.AppIdent); // For NLOG

            try
            {
                var host = CreateHostBuilder(args).Build();
                host.Run();

                GlobalParameters.MainRetCode = (int)MainRetCodes.OK;
                logger.Warn($"MTF exiting with exit code {GlobalParameters.MainRetCode}.");
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();

                return GlobalParameters.MainRetCode;
            }
            catch (Exception ex)
            {
                switch (ex.GetType().Name)
                {
                    case "MTFShutdown":
                        logger.Warn($"Application shutdown requested - {ex.Message}.");
                        GlobalParameters.MainRetCode = (int)MainRetCodes.Shutdown;
                        break;
                    default:
                        logger.Error($"Unhandled {ex.GetType().Name} exception '{ex.Message}' happend.");
                        GlobalParameters.MainRetCode = (int)MainRetCodes.UnhaltedException;
                        break;
                }
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }

            return GlobalParameters.MainRetCode;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((hostContext, options) => {
                        if (hostContext.HostingEnvironment.IsDevelopment())
                        {
                            options.AddServerHeader = true;
                        }
                        else
                        {
                            options.AddServerHeader = false;
                        };
                        options.Listen(IPAddress.Any, GlobalParameters._hostHTTPPort,
                                       listenOptions => {
                                                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                                        }
                                      );
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}

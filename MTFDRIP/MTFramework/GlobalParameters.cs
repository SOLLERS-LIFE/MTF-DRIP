using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Reflection;
using System.Collections.ObjectModel;

namespace MTFramework.Utilities
{
    // All parameters needed not once (obtained from correspondent
    // entries in appsettings.json
    public enum MainRetCodes
    {
        OK = 0,
        DBsSeedingProblem = -1,
        Shutdown = -2,
        Restart = -3,
        UnhaltedException = -4
    }
    public static class GlobalParameters
    {
        public static int MainRetCode { get; set; } = (int)MainRetCodes.OK;
        public static string AppIdent { get; set; }
        public static bool _isDevelopment { get; set; }
        public static int _appDB_ConnectionTimeout { get; set; }
        public static int _hostHTTPPort { get; set; }
        private static ILoggerFactory _loggerFactory { get; set; }
        public static ILogger CreateLogger<T>() => _loggerFactory.CreateLogger<T>();
        public static ILogger CreateLogger(string categoryName) => _loggerFactory.CreateLogger(categoryName);
        public static void setLoggerFactory(ILoggerFactory lf)
        {
            _loggerFactory = lf;
        }

        // Trick to find if in migration routins
        // or any other external actions
        public static bool IsStartedWithMain { get; set; } = false;
        public static AssemblyInfo _appInfo { get; set; }
        public static void Fulfill(IConfiguration configuration,
                                   IWebHostEnvironment env
                                  )
        {

            _appInfo = new AssemblyInfo();
            _isDevelopment = env.IsDevelopment();
            AppIdent = configuration.GetSection("Logging").GetValue<string>("AppIdent", "MTF-based App");
            _appDB_ConnectionTimeout = configuration.GetSection("applicationDB").GetValue<int>("connectionTimeout", 5);
            _hostHTTPPort = configuration.GetValue<int>("Host:httpPort",80);
        }
    }
}

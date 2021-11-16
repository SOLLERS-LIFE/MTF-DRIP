using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Diagnostics;

using MTFramework.Utilities;
using MTFDRIP.ApplicationDB.Data;

namespace MTFramework.Controllers
{
    /// <summary>
    /// MTFramework System Controller. May contains different useful operations, including application shutdown
    /// </summary>
    [ApiController]
    [Route("sysctl")]
    [Produces("application/json")]
    public class sysctlController : MTFControllerBase
    {
        private IHostApplicationLifetime _appLifetime { get; init; }
        private AppDB_Context _appdb { get; init; }
        public sysctlController(IHostApplicationLifetime appLifetime,
                                ILogger<sysctlController> logger,
                                AppDB_Context appdb)
            : base (logger)
        {
            _appLifetime = appLifetime;
            _appdb = appdb;
        }

        /// <summary>
        /// Application Shutdown. If use with k8s or podman with container "reload" - just restarts an app
        /// </summary>
        [HttpPost("shutdown")]
        public IActionResult OnPostAsync()
        {
            try
            {
                _logger.LogWarning("Shutdown was requested by user");
                GlobalParameters.MainRetCode = (int)MainRetCodes.Shutdown;
                _appLifetime.StopApplication();

                return Ok(new { msg = "Shutdown started. Enjoy yourself!" });
            }
            catch (Exception ex)
            {
                return exceptionResult(ex, " - during shutdown attempt");
            }
        }
        /// <summary>
        /// Rollback database to initial state, useful for testing
        /// </summary> 
        [HttpPost("rollupDatabase")]
        public IActionResult OnPostClearTestDataAsync()
        {
            try
            {
                _logger.LogWarning("rollupDatabase was requested by user");

                AppDBInitializer.DoIt(_appdb).Wait();

                return Ok(new { msg = "rollupDatabase finished."});
            }
            catch (Exception ex)
            {
                return exceptionResult(ex, " - during rollupDatabase attempt");
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("error")]
        [HttpGet("error")]
        [HttpPut("error")]
        [HttpDelete("error")]
        public IActionResult OnError()
        {
            try
            {
                var exceptionDscr =
                    HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionDscr == null)
                {
                    return NotFound(new { msg = "direct request is not allowed" });
                }

                string msg = $"{(exceptionDscr?.Error).GetType().Name}"
                             + $" - {exceptionDscr?.Path}"
                             + $" {exceptionDscr?.Error.Message}";

                _logger.LogError(msg);

                // The idea is to produce status code in dependence of conditions
                int sCode = StatusCodes.Status500InternalServerError;

                return StatusCode(sCode, new { msg = msg });
            }
            catch (Exception ex)
            {
                return exceptionResult(ex, " - during error handler");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace MTFramework.Utilities
{
    public class MTFControllerBase : ControllerBase
    {
        protected ILogger _logger { get; init; }
        public MTFControllerBase(ILogger logger)
            : base()
        {
            _logger = logger;
        }

        protected IActionResult exceptionResult(Exception ex, string clarification = "")
        {
            var msg = $"exception {ex.GetType().Name} - {ex.Message}{clarification}.";
            _logger.LogWarning(msg);
            return StatusCode(StatusCodes.Status500InternalServerError, new { msg = msg });
        }
    }
}

using Demo.Backend.Logging.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Demo.Backend.Logging
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v1/logs")]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpPost("client")]
        public ActionResult PostClientLog(NgxLogDto postLog)
        {
            if (postLog == null)
            {
                return BadRequest();
            }

            _logger.Log(postLog.Level.ToLogLevel(), $"{postLog.Message} {{@ClientLog}}", postLog);

            return NoContent();
        }
    }
}
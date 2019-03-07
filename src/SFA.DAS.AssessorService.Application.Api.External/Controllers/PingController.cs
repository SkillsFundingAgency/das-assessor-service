using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PingController : Controller
    {
        [HttpGet("/Ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}
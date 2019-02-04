using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    public class PingController : Controller
    {
        [HttpGet("/Ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}
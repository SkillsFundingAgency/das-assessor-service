using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class RegisterUpdateController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RegisterUpdateController> _logger;

        public RegisterUpdateController(IMediator mediator, ILogger<RegisterUpdateController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            _logger.LogInformation("Received Update Request");
            await _mediator.Send(new RegisterUpdateRequest());

            return Ok();
        }
    }
}
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/register-import")]
    public class RegisterImportController : Controller
    {
        private readonly ILogger<RegisterImportController> _logger;
        private readonly IMediator _mediator;

        public RegisterImportController(IMediator mediator,
            ILogger<RegisterImportController> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost(Name = "Import")]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Import()
        {
            _logger.LogInformation("Received Update Request");
            var response = await _mediator.Send(new RegisterUpdateRequest());

            return Ok(response);
        }
    }
}
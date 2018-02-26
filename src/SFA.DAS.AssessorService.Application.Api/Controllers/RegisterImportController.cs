using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/register-import")]
    public class RegisterImportController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RegisterImportController> _logger;

        public RegisterImportController(IMediator mediator,
            ILogger<RegisterImportController> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost(Name = "Import")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Import()
        {
            _logger.LogInformation("Received Update Request");
            await _mediator.Send(new RegisterUpdateRequest());

            return Ok();
        }
    }
}
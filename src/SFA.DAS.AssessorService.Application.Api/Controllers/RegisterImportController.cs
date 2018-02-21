using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Route("api/v1/register-import")]
    public class RegisterImportController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly ILogger<OrganisationController> _logger;

        public RegisterImportController(IMediator mediator,
            IStringLocalizer<OrganisationController> localizer,
            ILogger<OrganisationController> logger
        )
        {
            _mediator = mediator;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpPost(Name = "Import")]
        public async Task<IActionResult> Import()
        {
            _logger.LogInformation("Received Update Request");
            await _mediator.Send(new RegisterUpdateRequest());

            return Ok();
        }
    }
}
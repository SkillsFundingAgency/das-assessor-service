using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/certificate")]
    public class CertificateController : Controller
    {
        private readonly IMediator _mediator;

        public CertificateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "start")]
        public async Task<IActionResult> Start([FromBody] GetCertificateRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
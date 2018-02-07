using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class RegisterUpdateController : Controller
    {
        private readonly IMediator _mediator;

        public RegisterUpdateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _mediator.Send(new RegisterUpdateRequest());

            return Ok();
        }
    }
}
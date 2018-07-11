using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities.ao;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    //[Route("api/ao")]
    [ValidateBadRequest]
    public class RegisterController: Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator, ILogger<RegisterController> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }



        [HttpGet("/api/ao/organisation-types", Name = "GetOrganisationTypes")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<EpaOrganisationType>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationTypes()
        { 
            _logger.LogInformation("Get Organisation Types");
            var response = await _mediator.Send(new GetOrganisationsRequest());

            return Ok(response);
        }
        
    }

   
}

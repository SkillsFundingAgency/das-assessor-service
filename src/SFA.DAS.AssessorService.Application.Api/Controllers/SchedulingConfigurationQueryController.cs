using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/schedulingconfiguration")]
    [ValidateBadRequest]
    public class SchedulingConfigurationQueryController : Controller
    {
        private readonly IMediator _mediator;

        public SchedulingConfigurationQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetSchedulingConfiguration")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ScheduleConfigurationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetSchedulingConfiguration()
        {
            var scheduleConfigurationResponse = await _mediator.Send(new GetScheduleConfigurationRequest());
            if (scheduleConfigurationResponse == null)
                return NoContent();
            return Ok(scheduleConfigurationResponse);
        }
    }
}
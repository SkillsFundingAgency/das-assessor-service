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
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/")]
    [ValidateBadRequest]
    public class SettingController : Controller
    {
        private readonly IMediator _mediator;
      
        public SettingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPut("assessor-setting/{name}/{value}", Name = "SetAssessorSetting")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SetAssessorSetting(string name, string value)
        {
            await _mediator.Send(new SetSettingRequest { Name = name, Value = value });
            return Ok();
        }        
    }
}
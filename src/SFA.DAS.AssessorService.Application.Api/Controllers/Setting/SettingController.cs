using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.Validators;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/")]
    [ValidateBadRequest]
    public class SettingController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SetSettingRequestValidator _setSettingValidator;

        public SettingController(IMediator mediator, SetSettingRequestValidator setSettingRequestValidator)
        {
            _mediator = mediator;
            _setSettingValidator = setSettingRequestValidator;
        }
        
        [HttpPut("assessor-setting/{name}/{value}", Name = "SetAssessorSetting")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SetAssessorSetting(string name, string value)
        {
            var request = new SetSettingRequest { Name = name, Value = value };
            
            var validationResult = await _setSettingValidator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                var setSettingResult = await _mediator.Send(request);
                if(setSettingResult == SetSettingResult.Created)
                {
                    return CreatedAtRoute(nameof(SettingQueryController.GetAssessorSetting), new { name });
                }

                return Ok();
            }
            
            return BadRequest(validationResult.Errors.First()?.ErrorMessage);
        }        
    }
}
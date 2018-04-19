using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/emailTemplates")]
    [ValidateBadRequest]
    public class EMailTemplateQueryController : Controller
    {
        private readonly IMediator _mediator;

        public EMailTemplateQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{templateName}", Name = "GetEMailTemplate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EMailTemplate))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEMailTemplate(string templateName)
        {
            var emailTemplate = await _mediator.Send(new GetEMailTemplateRequest { TemplateName = templateName });
            if (emailTemplate == null)
                throw new ResourceNotFoundException();
            return Ok(emailTemplate);
        }
    }
}
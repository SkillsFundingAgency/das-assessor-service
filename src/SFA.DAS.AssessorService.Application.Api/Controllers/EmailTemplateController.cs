using System;
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

using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/emailTemplates")]
    [ValidateBadRequest]
    public class EmailTemplateController : Controller
    {
        private readonly ILogger<EmailTemplateController> _logger;
        private readonly IMediator _mediator;

        public EmailTemplateController( IMediator mediator,ILogger<EmailTemplateController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost(Name = "SendEmailToContact")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SendEmailToContact(
            [FromBody] SendEmailRequest sendEmailRequest)
        {
            _logger.LogInformation("Received Request To Send Email");

            await _mediator.Send(sendEmailRequest);

            return NoContent();
        }
    }
}
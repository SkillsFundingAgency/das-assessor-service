using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/review")]
    [ValidateBadRequest]
    public class ApplicationReviewController : Controller
    {
        private readonly ILogger<ApplicationReviewController> _logger;
        private readonly IMediator _mediator;

        public ApplicationReviewController(ILogger<ApplicationReviewController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("/Applications/{applicationId}/Sequences/{sequenceId}/Return")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task ReturnApplication(Guid applicationId, int sequenceId, [FromBody] ReturnApplicationRequest request)
        {
            _logger.LogInformation($"Received request to return application");
            await _mediator.Send(new AssessorService.Api.Types.Models.Apply.Review.ReturnApplicationRequest(applicationId, sequenceId, request.ReturnType));
        }

        public class ReturnApplicationRequest
        {
            public string ReturnType { get; set; }
        }
    }
}
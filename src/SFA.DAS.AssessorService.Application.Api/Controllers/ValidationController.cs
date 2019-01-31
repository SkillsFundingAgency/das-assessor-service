using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/validation/")]
    [ValidateBadRequest]
    public class ValidationController : Controller
    {
        private readonly ILogger<ValidationController> _logger;
        private readonly IMediator _mediator;

        public ValidationController(ILogger<ValidationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
       

        [HttpGet("is-email-format/{emailToValidate}", Name = "ValidateEmail")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateEmail(string emailToValidate)
        {
            return Ok(await _mediator.Send(new ValidationRequest{ValidationType = "email", ValidationString = emailToValidate}));
        }


        [HttpGet("is-not-empty/{stringToValidate}", Name = "ValidateRequired")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateRequired(string stringToValidate)
        {
            return Ok(await _mediator.Send(new ValidationRequest { ValidationType = "notEmpty", ValidationString = stringToValidate }));
        }

        [HttpGet("is-ukprn-format/{ukprnToValidate}", Name = "ValidateUkprn")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateUkprn(string ukprnToValidate)
        {
            return Ok(await _mediator.Send(new ValidationRequest { ValidationType = "ukprn", ValidationString = ukprnToValidate }));
        }
    }
}
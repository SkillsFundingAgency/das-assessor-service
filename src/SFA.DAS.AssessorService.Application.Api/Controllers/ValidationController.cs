﻿using System.Collections.Generic;
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
            return Ok(await _mediator.Send(new ValidationRequest{Type = "email", Value = emailToValidate}));
        }


        [HttpGet("is-not-empty/{stringToValidate}", Name = "ValidateRequired")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateRequired(string stringToValidate)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "notEmpty", Value = stringToValidate }));
        }

        [HttpGet("is-ukprn-format/{ukprnToValidate}", Name = "ValidateUkprn")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateUkprn(string ukprnToValidate)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "ukprn", Value = ukprnToValidate }));
        }

        [HttpGet("is-uln-format/{ulnToValidate}", Name = "ValidateUln")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateUln(string ulnToValidate)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "uln", Value = ulnToValidate }));
        }

        [HttpGet("is-minimum-length-or-more/{stringToValidate}/{minimumLength}", Name = "ValidateMinimumLength")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateMinimumLength(string stringToValidate, int minimumLength)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "minimumLength", Value = stringToValidate, MatchCriteria = minimumLength.ToString() }));
        }

        [HttpGet("is-maximum-length-or-less/{stringToValidate}/{maximumLength}", Name = "ValidateMaximumLength")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateMaximumLength(string stringToValidate, int maximumLength)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "maximumLength", Value = stringToValidate, MatchCriteria = maximumLength.ToString() }));
        }

        [HttpGet("is-valid-date/{dateToCheck}", Name = "ValidateDate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateDate(string dateToCheck)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "validDate", Value = dateToCheck }));
        }

        [HttpGet("is-date-today-or-in-future/{dateToCheck}", Name = "ValidateDateTodayOrInFuture")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateDateTodayOrInFuture(string dateToCheck)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "dateIsTodayOrInFuture", Value = dateToCheck }));
        }

        [HttpGet("is-date-today-or-in-past/{dateToCheck}", Name = "ValidateDateTodayOrInPast")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateDateTodayOrInPast(string dateToCheck)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "dateIsTodayOrInPast", Value = dateToCheck }));
        }

        [HttpGet("is-organisation-id-format/{organisationIdToValidate}", Name = "ValidateOrganisationId")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateOrganisationId(string organisationIdToValidate)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "organisationId", Value = organisationIdToValidate }));
        }

        [HttpGet("is-company-number-format/{companyNumberToCheck}", Name = "ValidateCompanyNumber")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateCompanyNumber(string companyNumberToCheck)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "companyNumber", Value = companyNumberToCheck }));
        }

        [HttpGet("is-charity-number-format/{charityNumberToCheck}", Name = "ValidateCharityNumber")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ValidateCharityNumber(string charityNumberToCheck)
        {
            return Ok(await _mediator.Send(new ValidationRequest { Type = "charityNumber", Value = charityNumberToCheck }));
        }

        
    }
}
using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/ao/assessment-organisations")]
    [ValidateBadRequest]
    public class RegisterValidationController : Controller
    {
        private readonly ILogger<RegisterValidationController> _logger;
        private readonly IMediator _mediator;

        public RegisterValidationController(IMediator mediator, ILogger<RegisterValidationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("validate-new", Name = "CreateEpaOrganisationValidate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisationValidate(CreateEpaOrganisationValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Validation of creating new Organisation");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(ex);
            }
        }

        [HttpGet("validate-existing", Name = "UpdateEpaOrganisationValidate")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateOrganisationValidate(UpdateEpaOrganisationValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Validation of existing Organisation");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(ex);
            }
        }

        [HttpGet("contacts/validate-new", Name = "CreateEpaContactValidate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        public async Task<IActionResult> CreateContactValidate(CreateEpaContactValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Validation of creating new contact");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(ex);
            }
        }

        [HttpGet("contacts/validate-existing", Name = "UpdateEpaContactValidate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        public async Task<IActionResult> CreateContactValidate(UpdateEpaOrganisationContactValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Validation of creating new contact");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(ex);
            }
        }

        [HttpGet("standards/validate-new", Name = "CreateEpaOrganisationStandardValidate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisationStandardValidation(CreateEpaOrganisationStandardValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Validation of creating new organisation standard");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(ex);
            } 
        }

        [HttpGet("standards/validate-existing", Name = "UpdateEpaOrganisationStandardValidate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateOrganisationStandardValidation(UpdateEpaOrganisationStandardValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Validation of creating new organisation standard");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(ex);
            }

        }

        [HttpGet("standards/validate/search/{searchstring}", Name = "SearchStandardsValidate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        public async Task<IActionResult> SearchStandardsValidate(string searchstring)
        {
            return await SearchStandardsValidateSearchstring(searchstring?.Trim());
        }

        [HttpGet("standards/validate/search", Name = "SearchStandardsValidateEmptyString")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        public async Task<IActionResult> SearchStandardsValidateEmptyString()
        {
            return await SearchStandardsValidateSearchstring(string.Empty);
        }


        private async Task<IActionResult> SearchStandardsValidateSearchstring(string searchstring)
        {
            try
            {
                var request = new SearchStandardsValidationRequest { Searchstring = searchstring };
                _logger.LogInformation("Validation search standards");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(ex);
            }
        }
    }
}
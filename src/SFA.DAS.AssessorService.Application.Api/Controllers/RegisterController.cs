using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{

    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/ao/assessment-organisations")]
    [ValidateBadRequest]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator, ILogger<RegisterController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpPost(Name = "CreateEpaOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateEpaOrganisationRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Organisation");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationResponse(result));
            }
            
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation [{ex.Message}]");
                return Conflict(new EpaOrganisationResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }


        [HttpGet("validate",Name = "CreateEpaOrganisationValidate")]
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


        [HttpPut(Name = "UpdateEpaOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Gone, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateEpaOrganisation([FromBody] UpdateEpaOrganisationRequest request)
        {
            try
            {
                _logger.LogInformation($@"Updating Organisation [{request.OrganisationId}]");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationResponse(result));
            }

            catch (NotFound ex)
            {
                _logger.LogError($@"Record is not available for organisation ID: [{request.OrganisationId}]");
                return NotFound(new EpaOrganisationResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPost("contacts", Name = "CreateEpaOrganisationContact")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(EpaContact))]
        public async Task<IActionResult> CreateOrganisationContact([FromBody] CreateOrganisationContactRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Organisation Contact");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationContactResponse(result));
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation/email [{request.EndPointAssessorOrganisationId}, {request.Email}]");
                return Conflict(new EpaOrganisationContactResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationContactResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPost("standards",Name = "CreateEpaOrganisationStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationStandard))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisationStandard([FromBody] CreateEpaOrganisationStandardRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Organisation Standard");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationStandardResponse(result));
            }
            catch (NotFound ex)
            {
                _logger.LogError($@"Record is not available for organisation / standard: [{request.OrganisationId}, {request.StandardCode}]");
                return NotFound(new EpaOrganisationStandardResponse(ex.Message));
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation/standard [{request.OrganisationId}, {request.StandardCode}]");
                return Conflict(new EpaOrganisationStandardResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaOrganisationStandardResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPut("standards", Name = "UpdateEpaOrganisationStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationStandard))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateOrganisationStandard([FromBody] UpdateEpaOrganisationStandardRequest request)
        {
            try
            {
                _logger.LogInformation($@"Updating Organisation Standard [{request.OrganisationId}, {request.StandardCode}]");
                var result = await _mediator.Send(request);
                return Ok(new EpaOrganisationStandardResponse(result));
            }      
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest(new EpaOrganisationStandardResponse(ex.Message));
            }
        }
    }
}

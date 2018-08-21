using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{

    [Authorize]
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisation))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateEpaOrganisationRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Organisation");
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation [{request.OrganisationId}]");
                return Conflict(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }


        [HttpPut(Name = "UpdateEpaOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisation))]
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
                return Ok(result);
            }

            catch (NotFound ex)
            {
                _logger.LogError($@"Record is not available for organisation ID: [{request.OrganisationId}]");
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
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
                return Ok(result);
            }
            catch (NotFound ex)
            {
                _logger.LogError($@"Record is not available for organisation / standard: [{request.OrganisationId}, {request.StandardCode}]");
                return NotFound(ex.Message);
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation/standard [{request.OrganisationId}, {request.StandardCode}]");
                return Conflict(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

    }
}

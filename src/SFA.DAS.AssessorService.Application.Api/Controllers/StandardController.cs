using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Web.Staff.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/ao/update-standards")]
    [ValidateBadRequest]
    public class StandardController : Controller
    {
        private readonly ILogger<StandardController> _logger;
        private readonly IMediator _mediator;
        private readonly IStandardService _standardService;
        //private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;


        public StandardController(ILogger<StandardController> logger, IMediator mediator, IStandardService standardService)
        {
            _logger = logger;
            _mediator = mediator;
            _standardService = standardService;
        }

        [HttpPost(Name = "GatherAndStoreStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GatherStandardsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GatherAndStoreStandards([FromBody] GatherStandardsRequest request)
        {

            var processDetails = string.Empty;
            var standards = await _standardService.GatherAllStandardDetails();

//            try
//            {
//                _logger.LogInformation("Creating new Organisation");
//                var result = await _mediator.Send(request);
//                return Ok(new EpaOrganisationResponse(result));
//            }
//
//            catch (AlreadyExistsException ex)
//            {
//                _logger.LogError($@"Record already exists for organisation [{ex.Message}]");
//                return Conflict(new EpaOrganisationResponse(ex.Message));
//            }
//            catch (BadRequestException ex)
//            {
//                _logger.LogError(ex.Message);
//                return BadRequest(new EpaOrganisationResponse(ex.Message));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
//                return BadRequest();
//            }

            processDetails = $"{processDetails}processing complete; ";
            return Ok(new GatherStandardsResponse(processDetails));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Data.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/mergeorganisations")]
    public class MergeOrganisationsController : Controller
    {
        private readonly ILogger<MergeOrganisationsController> _logger;
        private readonly IMediator _mediator;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;

        public MergeOrganisationsController(
            IEMailTemplateQueryRepository eMailTemplateQueryRepository,
            ILogger<MergeOrganisationsController> logger,
            IMediator mediator
        )
        {
            _logger = logger;
            _mediator = mediator;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
        }

        [HttpPost(Name = "MergeOrganisations")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> MergeOrganisations([FromBody] MergeOrganisationsRequest request)
        {
            _logger.LogInformation("Received Merge Organisation Request");

            var mergeOrganisationsRequest = new Handlers.OrganisationHandlers.MergeOrganisationsRequest
            {
                PrimaryEndPointAssessorOrganisationId = request.PrimaryEndPointAssessorOrganisationId,
                SecondaryEndPointAssessorOrganisationId = request.SecondaryEndPointAssessorOrganisationId,
                SecondaryStandardsEffectiveTo = request.SecondaryStandardsEffectiveTo,
                ActionedByUser = request.ActionedByUser
            };

            var mergeOrganisation = await _mediator.Send(mergeOrganisationsRequest);

            // Send emails on success
            if (null != mergeOrganisation && mergeOrganisation.Status == Services.MergeOrganisationStatus.Completed)
            {
                try
                {
                    var primaryEmailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.MergeConfirmationForPrimaryEpao);
                    if (null != primaryEmailTemplate)
                    {
                        await _mediator.Send(new SendEmailRequest(mergeOrganisation.PrimaryOrganisationEmail, primaryEmailTemplate,
                            new
                            {
                                secondaryEPAO = mergeOrganisation.SecondaryEndPointAssessorOrganisationName,
                                contactName = mergeOrganisation.PrimaryContactName,
                                effectiveToDate = mergeOrganisation.SecondaryEPAOEffectiveTo.ToString("dd MMMM yyyy")
                            }));
                    }

                    var secondaryEmailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.MergeConfirmationForSecondaryEpao);
                    if (null != secondaryEmailTemplate)
                    {
                        await _mediator.Send(new SendEmailRequest(mergeOrganisation.SecondaryOrganisationEmail, secondaryEmailTemplate,
                            new
                            {
                                primaryEPAO = mergeOrganisation.PrimaryEndPointAssessorOrganisationName,
                                contactName = mergeOrganisation.SecondaryContactName,
                                effectiveToDate = mergeOrganisation.SecondaryEPAOEffectiveTo.ToString("dd MMMM yyyy")
                            }));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send merge confirmation email");
                }
            }

            if(null != mergeOrganisation)
            {
                return CreatedAtRoute("GetMergeOrganisation", new { id = mergeOrganisation.Id });
            }
            return new StatusCodeResult(500);
        }


        [HttpGet(Name = "GetMergeOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MergeLogEntry))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetMergeOrganisation(int id)
        {
            _logger.LogInformation("Received Get Merge Organisation Request");

            var request = new Handlers.OrganisationHandlers.GetMergeOrganisationRequest() { Id = id };

            var response = await _mediator.Send(request);
            if(null == response)
            {
                return NotFound();
            }

            return new OkObjectResult(response);
        }

        [HttpGet("log", Name = "GetMergeLog")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MergeLogEntry))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetMergeLog(int? pageSize, int? pageIndex, string orderBy, string orderDirection, string primaryEPAOId, string secondaryEPAOId)
        {
            var request = new Handlers.OrganisationHandlers.GetMergeLogRequest()
            { 
                PageSize = pageSize, 
                PageIndex = pageIndex, 
                OrderBy = orderBy, 
                OrderDirection = orderDirection, 
                PrimaryEPAOId = primaryEPAOId, 
                SecondaryEPAOId = secondaryEPAOId,
                Status = "Completed"
            };
            
            var response = await _mediator.Send(request);
            return new OkObjectResult(response);
        }
    }
}
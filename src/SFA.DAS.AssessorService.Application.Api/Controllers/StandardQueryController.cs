using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/standards")]
    [ValidateBadRequest]
    public class StandardQueryController : Controller
    {

        private readonly ILogger<StandardQueryController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public StandardQueryController(IMediator mediator, ILogger<StandardQueryController> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the standards which the given organisation is assessing.
        /// </summary>
        /// <param name="epaoId">The organisation for which standards are returned</param>
        /// <param name="requireAtLeastOneVersion">When true the organisation must be assessing atleast one version of the standard</param>
        /// <param name="pageIndex">The index of the page of results to return</param>
        /// <param name="pageSize">The number of results to return in a single page</param>
        /// <returns></returns>
        [HttpGet("{epaoId}", Name = "GetEpaoRegisteredStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoRegisteredStandards(string epaoId, bool? requireAtLeastOneVersion = true, int? pageIndex = 1, int? pageSize = 10)
        {
            _logger.LogInformation($"Received request to retrieve Standards for Organisation {epaoId}");
            return Ok(await _mediator.Send(new GetEpaoRegisteredStandardsRequest(epaoId, requireAtLeastOneVersion.Value, pageIndex.Value, pageSize.Value)));
        }

        [HttpGet("pipelines/{epaoId}", Name = "GetEpaoPipelineStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoPipelineStandards(string epaoId, string standardFilterId, string providerFilterId, string epaDateFilterId, string orderBy, string orderDirection,int pageSize, int? pageIndex = null)
        {
            _logger.LogInformation($"Received request to retrieve pipeline for standards of the organisation {epaoId}");
            if (string.IsNullOrWhiteSpace(epaoId))
            {
                return BadRequest();
            }
            var normalisedPageIndex = (pageIndex == null || pageIndex == 0) ? 1 : pageIndex;
            return Ok(await _mediator.Send(new EpaoPipelineStandardsRequest(epaoId, standardFilterId, providerFilterId, epaDateFilterId, orderBy, orderDirection,normalisedPageIndex, pageSize)));
        }

        [HttpGet("pipelines/{epaoId}/filters", Name = "GetEpaoPipelineStandardsFilters")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoPipelineStandardsFilters(string epaoId)
        {
            _logger.LogInformation($"Received request to retrieve pipeline filters for organisation {epaoId}");
            if (string.IsNullOrWhiteSpace(epaoId))
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EpaoPipelineStandardsFiltersRequest(epaoId)));
        }

        [HttpGet("pipelines/extract/{epaoId}", Name = "GetEpaoPipelineStandardsExtract")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoPipelineStandardsExtract(string epaoId, string standardFilterId, string providerFilterId, string epaDateFilterId)
        {
            _logger.LogInformation($"Received request to extract pipeline for standards of the organisation {epaoId}");
            return Ok(await _mediator.Send(new EpaoPipelineStandardsExtractRequest(epaoId, standardFilterId, providerFilterId, epaDateFilterId)));
        }

        [HttpGet("{standardCode}/organisations")]
        [ProducesResponseType(typeof(List<OrganisationStandardResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async  Task<IActionResult> GetEpaosByStandard(int standardCode)
        {
            if (standardCode == 0)
            {
                return BadRequest();
            }

            var epaOrganisationResponse = await _mediator.Send(new GetEpaOrganisationsByStandardQuery
            {
                Standard = standardCode
            });

            if (!epaOrganisationResponse.EpaOrganisations.Any())
            {
                return NotFound();
            }

            var result = epaOrganisationResponse.EpaOrganisations.Select(_mapper.Map<OrganisationStandardResponse>).ToList();

            return Ok(result);

        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Attributes;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Ilr;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/search")]
    [ValidateBadRequest]
    public class SearchController : Controller
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IIlrRepository _ilrRepository;

        public SearchController(IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository, IIlrRepository ilrRepository)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationRepository = organisationRepository;
            _ilrRepository = ilrRepository;
        }

        [HttpPost(Name = "Search")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<SearchResult>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Search([FromBody]SearchQuery searchQuery)
        {
            var epaOrgId = await _organisationRepository.GetByUkPrn(searchQuery.UkPrn);

            var standards = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(epaOrgId
                    .EndPointAssessorOrganisationId);

            var searchResult =
                Mapper.Map<List<SearchResult>>(_ilrRepository.Search(new SearchRequest()
                {
                    FamilyName = searchQuery.Surname,
                    Uln = searchQuery.Uln,
                    StandardIds = standards.Select(s => s.StandardCode).ToList()
                }));

            foreach (var ilrResult in searchResult)
            {
                ilrResult.Standard = (await _assessmentOrgsApiClient.GetStandard(ilrResult.StdCode)).Title;
            }

            return Ok(searchResult);
        }
    }
}
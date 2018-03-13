using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Ilr;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Route("api/v1/search")]
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
        public async Task<IActionResult> Search([FromBody]SearchQuery searchQuery)
        {
            var epaOrgId = await _organisationRepository.GetByUkPrn(int.Parse(searchQuery.UkPrn));

            var standards = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(epaOrgId
                    .EndPointAssessorOrganisationId);

            var searchResult = _ilrRepository.Search(new SearchRequest()
            {
                FamilyName = searchQuery.Surname,
                Uln = searchQuery.Uln,
                StandardIds = standards.Select(s => s.StandardCode).ToList()
            });
            
            foreach (var ilrResult in searchResult)
            {
                ilrResult.Standard = (await _assessmentOrgsApiClient.GetStandard(ilrResult.StdCode)).Title;
            }

            return Ok(searchResult);
        }
    }
}
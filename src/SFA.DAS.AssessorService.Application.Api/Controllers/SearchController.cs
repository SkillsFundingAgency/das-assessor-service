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
        private readonly IIlrApiClient _ilrApi;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;

        public SearchController(IIlrApiClient ilrApi, IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository)
        {
            _ilrApi = ilrApi;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationRepository = organisationRepository;
        }

        [HttpPost(Name = "Search")]
        public async Task<IActionResult> Search([FromBody]SearchQuery searchQuery)
        {
            var epaOrgId = await _organisationRepository.GetByUkPrn(int.Parse(searchQuery.UkPrn));

            var standards = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(epaOrgId
                    .EndPointAssessorOrganisationId);

            var result = await _ilrApi.Search(new IlrSearchRequest()
            {
                Surname = searchQuery.Surname,
                Uln = searchQuery.Uln,
                StandardIds = standards.Select(s => s.StandardCode)
            });

            foreach (var ilrResult in result.Results)
            {
                ilrResult.Standard = (await _assessmentOrgsApiClient.GetStandard(ilrResult.StandardId)).Title;
            }

            return Ok(result);
        }
    }
}
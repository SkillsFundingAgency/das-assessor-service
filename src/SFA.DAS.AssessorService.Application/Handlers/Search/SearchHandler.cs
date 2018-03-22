using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    public class SearchHandler : IRequestHandler<SearchQuery, List<SearchResult>>
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly ICertificateRepository _certificateRepository;

        public SearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationRepository = organisationRepository;
            _ilrRepository = ilrRepository;
            _certificateRepository = certificateRepository;
        }
        public async Task<List<SearchResult>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var thisEpao = await _organisationRepository.GetByUkPrn(request.UkPrn);

            var theStandardsThisEpaoProvides = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(thisEpao
                .EndPointAssessorOrganisationId);

            var ilrResults = await _ilrRepository.Search(new SearchRequest
            {
                FamilyName = request.Surname,
                Uln = request.Uln,
                StandardIds = theStandardsThisEpaoProvides.Select(s => s.StandardCode).ToList()
            });

            ilrResults = await RemoveCompletedStandards(request, ilrResults);

            ilrResults = ilrResults.OrderByDescending(ilr => ilr.LearnStartDate).Take(1);

            var searchResult = Mapper.Map<List<SearchResult>>(ilrResults);

            foreach (var ilrResult in searchResult)
            {
                // Yes, I know this is calling out to an API in a tight loop, but there will be very few searchResults.  Usually only one.
                // Obviously if this does become a perf issue, then it'll need changing.
                ilrResult.Standard = (await _assessmentOrgsApiClient.GetStandard(ilrResult.StdCode)).Title;
            }

            return searchResult;
        }

        private async Task<IEnumerable<Ilr>> RemoveCompletedStandards(SearchQuery request, IEnumerable<Ilr> ilrResults)
        {
            var completedStandards = (await _certificateRepository.GetCompletedCertificatesFor(request.Uln))
                .Select(c => c.StandardCode.ToString()).ToList();

            return ilrResults.Where(r => !completedStandards.Contains(r.StdCode));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    static class SearchResultExtensions
    {
        public static List<SearchResult> PopulateStandards(this List<SearchResult> searchResults, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            foreach (var searchResult in searchResults)
            {
                // Yes, I know this is calling out to an API in a tight loop, but there will be very few searchResults.  Usually only one.
                // Obviously if this does become a perf issue, then it'll need changing.
                var standard = assessmentOrgsApiClient.GetStandard(searchResult.StdCode).Result;
                searchResult.Standard = standard.Title;
                searchResult.Level = standard.Level;
            }

            return searchResults;
        }

        public static List<SearchResult> MatchUpExistingCompletedStandards(this List<SearchResult> searchResults, SearchQuery request, ICertificateRepository certificateRepository)
        {
            var completedCertificates = certificateRepository.GetCompletedCertificatesFor(request.Uln).Result;

            foreach (var searchResult in searchResults.Where(r => completedCertificates.Select(s => s.StandardCode).Contains(r.StdCode)))
            {
                var certificate = completedCertificates.Single(s => s.StandardCode == searchResult.StdCode);
                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                searchResult.CertificateReference = certificate.CertificateReference;
                searchResult.OverallGrade = certificateData.OverallGrade;
            }

            return searchResults;
        }
    }
}
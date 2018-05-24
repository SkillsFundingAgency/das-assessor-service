﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    static class SearchResultExtensions
    {
        public static List<SearchResult> PopulateStandards(this List<SearchResult> searchResults, IAssessmentOrgsApiClient assessmentOrgsApiClient, ILogger<SearchHandler> logger)
        {
            foreach (var searchResult in searchResults)
            {
                // Yes, I know this is calling out to an API in a tight loop, but there will be very few searchResults.  Usually only one.
                // Obviously if this does become a perf issue, then it'll need changing.
                logger.LogInformation("PopulateStandards Before Get Standard from api");
                var standard = assessmentOrgsApiClient.GetStandard(searchResult.StdCode).Result;
                logger.LogInformation("PopulateStandards After Get Standard from api");
                searchResult.Standard = standard.Title;
                searchResult.Level = standard.Level;
            }

            return searchResults;
        }

        public static List<SearchResult> MatchUpExistingCompletedStandards(this List<SearchResult> searchResults, SearchQuery request, ICertificateRepository certificateRepository, ILogger<SearchHandler> logger)
        {
            logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
            var completedCertificates = certificateRepository.GetCompletedCertificatesFor(request.Uln).Result;
            logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");
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
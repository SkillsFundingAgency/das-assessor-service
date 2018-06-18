using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
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

        public static List<SearchResult> MatchUpExistingCompletedStandards(this List<SearchResult> searchResults, SearchQuery request, ICertificateRepository certificateRepository, IContactQueryRepository contactRepository, ILogger<SearchHandler> logger)
        {
            logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
            var completedCertificates = certificateRepository.GetCompletedCertificatesFor(request.Uln).Result;
            logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");
            foreach (var searchResult in searchResults.Where(r => completedCertificates.Select(s => s.StandardCode).Contains(r.StdCode)))
            {
                var certificate = completedCertificates.Single(s => s.StandardCode == searchResult.StdCode);
                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                searchResult.CertificateReference = certificate.CertificateReference;
                searchResult.LearnStartDate = certificateData.LearningStartDate == DateTime.MinValue ? null : new DateTime?(certificateData.LearningStartDate) ;

                var certificateLogs = certificateRepository.GetCertificateLogsFor(certificate.Id).Result;
                logger.LogInformation("MatchUpExistingCompletedStandards After GetCertificateLogsFor");
                var submittedLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);
                if (submittedLogEntry == null) continue;
                
                var submittingContact = contactRepository.GetContact(submittedLogEntry.Username).Result;
                logger.LogInformation("MatchUpExistingCompletedStandards After GetContact");

                var searchingContact = contactRepository.GetContact(request.Username).Result;

                if (submittingContact.OrganisationId == searchingContact.OrganisationId)
                {
                    searchResult.ShowExtraInfo = true;
                    searchResult.OverallGrade = certificateData.OverallGrade;
                    searchResult.SubmittedBy = submittingContact.DisplayName; // This needs to be contact real name
                    searchResult.SubmittedAt = submittedLogEntry.EventTime.ToLocalTime(); // This needs to be local time 
                    searchResult.AchDate = certificateData.AchievementDate;
                }
                else
                {
                    searchResult.ShowExtraInfo = false;
                    searchResult.OverallGrade = "";
                    searchResult.SubmittedBy = ""; // This needs to be contact real name
                    searchResult.SubmittedAt = null; // This needs to be local time 
                    searchResult.LearnStartDate = null;
                    searchResult.AchDate = null;
                }
            }

            return searchResults;
        }
    }
}
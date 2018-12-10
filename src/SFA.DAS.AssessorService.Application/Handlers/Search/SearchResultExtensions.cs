using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    static class SearchResultExtensions
    {
        public static List<SearchResult> PopulateStandards(this List<SearchResult> searchResults, IAssessmentOrgsApiClient assessmentOrgsApiClient, ILogger<SearchHandler> logger)
        {
            var allStandards = assessmentOrgsApiClient.GetAllStandards().Result;

            foreach (var searchResult in searchResults)
            {
                MatchStandards(searchResult, allStandards, assessmentOrgsApiClient);
            }

            return searchResults;
        }

        public static List<SearchResult> CleanFromSearchResultAnyMarkedAsRemove(this List<SearchResult> searchResults)
        {
            for (var i = searchResults.Count - 1; i >= 0; i--)
                if(searchResults[i].RemoveFromCollection)
                    searchResults.RemoveAt(i);

            return searchResults;
        }

        private static void MatchStandards(SearchResult searchResult, IEnumerable<Standard> allStandards, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            var standard = allStandards.SingleOrDefault(s => s.StandardId == searchResult.StdCode.ToString()) ??
                           assessmentOrgsApiClient.GetStandard(searchResult.StdCode).Result;
            searchResult.Standard = standard.Title;
            searchResult.Level = standard.Level;
        }

        public static List<SearchResult> MatchUpExistingCompletedStandards(this List<SearchResult> searchResults, 
            SearchQuery request, ICertificateRepository certificateRepository, IContactQueryRepository contactRepository, ILogger<SearchHandler> logger)
        {
            logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
            var certificates = certificateRepository.GetSubmittedAndDraftCertificatesFor(request.Uln).Result;
            logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");
            foreach (var searchResult in searchResults.Where(r => certificates.Select(s => s.StandardCode).Contains(r.StdCode)))
            {
                DoCertificateMatchUp(searchResult, certificates,
                    request, certificateRepository, contactRepository, logger);
              
            }

            return searchResults;
        }

     
        public static SearchResult MatchUpFoundCertificate(this SearchResult searchResult, SearchQuery request,
            List<Certificate> certificateList, IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IContactQueryRepository contactRepository, ICertificateRepository certificateRepository, ILogger<SearchHandler> logger)
        {

            DoCertificateMatchUp(searchResult, certificateList,
                request, certificateRepository, contactRepository, logger);
            var allStandards = assessmentOrgsApiClient.GetAllStandards().Result;
            MatchStandards(searchResult, allStandards, assessmentOrgsApiClient);

            return searchResult;
        }


        private static void DoCertificateMatchUp(SearchResult searchResult, List<Certificate> certificateList,
            SearchQuery request, ICertificateRepository certificateRepository,
            IContactQueryRepository contactRepository, ILogger<SearchHandler> logger)
        {
            var certificate = certificateList.Single(s => s.StandardCode == searchResult.StdCode);
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            var certificateLogs = certificateRepository.GetCertificateLogsFor(certificate.Id).Result;
            var submittedLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);
            var createdLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Draft);

            if (submittedLogEntry == null)
            {
                if (createdLogEntry == null) return;
                logger.LogInformation("DoCertificateMatchUp - Draft certificate found ");
                var createdContact = contactRepository.GetContact(createdLogEntry.Username).Result ??
                                     contactRepository.GetContact(certificate.CreatedBy).Result;
                var lastUpdatedLogEntry =
                    certificateLogs.Aggregate((i1, i2) => i1.EventTime > i2.EventTime ? i1 : i2) ?? createdLogEntry;
                var lastUpdatedContact = contactRepository.GetContact(lastUpdatedLogEntry.Username).Result;
                
                var searchingContact = contactRepository.GetContact(request.Username).Result;

                if (createdContact == null || searchingContact == null ||
                    createdContact.OrganisationId != searchingContact.OrganisationId)
                {
                    searchResult.RemoveFromCollection = true;
                    return;
                }

                ApplyBasicCertificateInfo(searchResult, certificateData, certificate, true);
                ApplyExtraCertificateInfo(searchResult, true, certificateData.OverallGrade,
                    createdLogEntry.Username,
                    createdLogEntry.EventTime.UtcFromTimeZoneTime(),
                    certificateData.AchievementDate,
                    lastUpdatedContact != null ? lastUpdatedContact.DisplayName : lastUpdatedLogEntry.Username,
                    lastUpdatedLogEntry.EventTime.UtcToTimeZoneTime());
            }
            else
            {
                logger.LogInformation("DoCertificateMatchUp - Submitted certificate found ");
                var submittingContact = contactRepository.GetContact(submittedLogEntry.Username).Result ??
                                        contactRepository.GetContact(certificate.UpdatedBy).Result;
                var lastUpdatedLogEntry =
                    certificateLogs.Aggregate((i1, i2) => i1.EventTime > i2.EventTime ? i1 : i2) ?? submittedLogEntry;
                var lastUpdatedContact = contactRepository.GetContact(lastUpdatedLogEntry.Username).Result;
                var searchingContact = contactRepository.GetContact(request.Username).Result;
                //Apply Basic Info
                ApplyBasicCertificateInfo(searchResult, certificateData, certificate, false);
                if (submittingContact != null && submittingContact.OrganisationId == searchingContact.OrganisationId)
                {
                    //Use contact realname, local time for submitted at and updated by use contact real name
                    //Apply extra info
                    ApplyExtraCertificateInfo(searchResult, true, certificateData.OverallGrade,
                        submittingContact.DisplayName,
                        submittedLogEntry.EventTime,
                        certificateData.AchievementDate,
                        lastUpdatedContact != null ? lastUpdatedContact.DisplayName : lastUpdatedLogEntry.Username,
                        lastUpdatedLogEntry.EventTime);
                }
                else
                {
                    ApplyExtraCertificateInfo(searchResult, false, string.Empty, string.Empty, null, null, null, null);
                }
            }
        }

        private static void ApplyBasicCertificateInfo(SearchResult searchResult, CertificateData certificateData,
            Certificate certificate, bool isDraft)
        {
            searchResult.Uln = certificate.Uln;
            searchResult.FamilyName = certificateData.LearnerFamilyName;
            searchResult.GivenNames = certificateData.LearnerGivenNames;
            searchResult.StdCode = certificate.StandardCode;
            searchResult.UkPrn = certificate.ProviderUkPrn;
            searchResult.CertificateReference = isDraft ? null:certificate.CertificateReference; //Set to null since the Web front end expects a null if draft - Needs revisting
            searchResult.CertificateId = certificate.Id;
            searchResult.CertificateStatus = certificate.Status;
            searchResult.LearnStartDate = certificateData.LearningStartDate;
            searchResult.Option = certificateData.CourseOption;
        }

        private static void ApplyExtraCertificateInfo(SearchResult searchResult,
            bool showExtraInfo, string overallGrade, string submittedBy, DateTime? submittedAt, DateTime? achDate, string updatedBy, DateTime? updatedAt)
        {
            searchResult.ShowExtraInfo = showExtraInfo;
            searchResult.OverallGrade = overallGrade;
            searchResult.SubmittedAt = submittedAt?.UtcToTimeZoneTime();
            searchResult.SubmittedBy = submittedBy; // This needs to be contact real name
            searchResult.AchDate = achDate?.UtcToTimeZoneTime(); ;
            searchResult.UpdatedBy = updatedBy; // This needs to be contact real name
            searchResult.UpdatedAt = updatedAt?.UtcToTimeZoneTime(); ; // This needs to be local time
        }

    }
}
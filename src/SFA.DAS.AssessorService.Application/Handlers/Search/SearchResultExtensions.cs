﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    static class SearchResultExtensions
    {
        public static List<SearchResult> PopulateStandards(this List<SearchResult> searchResults, IStandardService standardService, ILogger<SearchHandler> logger)
        {
            var allStandards = standardService.GetAllStandards().Result;

            foreach (var searchResult in searchResults)
            {
                var standard = allStandards.SingleOrDefault(s => s.StandardId == searchResult.StdCode);
                if (standard == null)
                {
                    try
                    {
                        standard = standardService.GetStandard(searchResult.StdCode)?.Result;
                    }
                    catch (Exception e)
                    {
                        logger.LogInformation($"Failed to get standard for {searchResult.StdCode}, error message: {e.Message}");
                    }
                }
                searchResult.Standard = standard?.Title;
                searchResult.Level = standard?.StandardData.Level.GetValueOrDefault()??0;
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
                searchResult.CertificateId = certificate.Id;
                searchResult.CertificateStatus = certificate.Status;
                searchResult.LearnStartDate = certificateData.LearningStartDate;
                searchResult.Option = certificateData.CourseOption;

                var certificateLogs = certificateRepository.GetCertificateLogsFor(certificate.Id).Result;
                logger.LogInformation("MatchUpExistingCompletedStandards After GetCertificateLogsFor");
                var submittedLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);
                var createdLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Draft);
                if (request.IsPrivatelyFunded)
                {
                    var toBeApprovedLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.ToBeApproved);
                    if (toBeApprovedLogEntry == null)
                        continue;
                    submittedLogEntry = toBeApprovedLogEntry;
                }

                if (submittedLogEntry == null) continue;

                var submittingContact = contactRepository.GetContact(submittedLogEntry.Username).Result ?? contactRepository.GetContact(certificate.UpdatedBy).Result;
                var createdContact = contactRepository.GetContact(createdLogEntry?.Username).Result ?? contactRepository.GetContact(certificate.CreatedBy).Result;

                var lastUpdatedLogEntry = certificateLogs.Aggregate((i1, i2) => i1.EventTime > i2.EventTime ? i1 : i2) ?? submittedLogEntry;
                var lastUpdatedContact = contactRepository.GetContact(lastUpdatedLogEntry.Username).Result;

                logger.LogInformation("MatchUpExistingCompletedStandards After GetContact");

                var searchingContact = contactRepository.GetContact(request.Username).Result;

                if (submittingContact != null && submittingContact.OrganisationId == searchingContact.OrganisationId)
                {
                    searchResult.ShowExtraInfo = true;
                    searchResult.OverallGrade = certificateData.OverallGrade;
                    searchResult.SubmittedBy = submittingContact.DisplayName; // This needs to be contact real name
                    searchResult.SubmittedAt = submittedLogEntry.EventTime.UtcToTimeZoneTime(); // This needs to be local time 
                    searchResult.AchDate = certificateData.AchievementDate;
                    searchResult.UpdatedBy = lastUpdatedContact != null ? lastUpdatedContact.DisplayName : lastUpdatedLogEntry.Username; // This needs to be contact real name
                    searchResult.UpdatedAt = lastUpdatedLogEntry.EventTime.UtcToTimeZoneTime(); // This needs to be local time
                }
                else if (createdContact != null && searchingContact != null && createdContact.OrganisationId == searchingContact.OrganisationId)
                {
                    searchResult.ShowExtraInfo = true;
                    searchResult.OverallGrade = certificateData.OverallGrade;
                    searchResult.SubmittedBy = submittedLogEntry.Username; // This needs to be contact real name
                    searchResult.SubmittedAt = submittedLogEntry.EventTime.UtcToTimeZoneTime(); // This needs to be local time 
                    searchResult.AchDate = certificateData.AchievementDate;
                    searchResult.UpdatedBy = lastUpdatedContact != null ? lastUpdatedContact.DisplayName : lastUpdatedLogEntry.Username; // This needs to be contact real name
                    searchResult.UpdatedAt = lastUpdatedLogEntry.EventTime.UtcToTimeZoneTime(); // This needs to be local time
                }
                else
                {
                    searchResult.ShowExtraInfo = false;
                    searchResult.OverallGrade = "";
                    searchResult.SubmittedBy = "";
                    searchResult.SubmittedAt = null;
                    searchResult.LearnStartDate = null;
                    searchResult.AchDate = null;
                    searchResult.UpdatedBy = null;
                    searchResult.UpdatedAt = null; 
                }
            }

            return searchResults;
        }
    }
}
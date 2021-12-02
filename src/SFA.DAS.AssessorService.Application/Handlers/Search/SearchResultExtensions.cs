using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    public static class SearchResultExtensions
    {
        public static List<SearchResult> PopulateStandards(this List<SearchResult> searchResults, IStandardService standardService, ILogger<SearchHandler> logger)
        {
            var allStandards = standardService.GetAllStandardVersions().Result;
            var allOptions = standardService.GetAllStandardOptions().Result;

            foreach (var searchResult in searchResults)
            {
                // If the learner contains version and option, then it's come from commitments
                // Therefore we don't want to give the UI the impression that a choice is needed
                // So we enforce a single version and option selection so it displays on the results page.
                if (searchResult.VersionConfirmed && !string.IsNullOrWhiteSpace(searchResult.StandardUId))
                {
                    var learnerStandard = allStandards.FirstOrDefault(s => s.StandardUId == searchResult.StandardUId);

                    if (learnerStandard == null)
                    {
                        logger.LogInformation($"Failed to get standard for {searchResult.StandardUId}");
                    }
                    else
                    {
                        PopulateSingleVersionResult(learnerStandard, allOptions, searchResult);
                    }
                    continue;
                }

                // Otherwise, we fall back to traditional processing populating versions.
                var standards = allStandards.Where(s => s.LarsCode == searchResult.StdCode)
                    .OrderByDescending(o => o.VersionMajor).ThenByDescending(m => m.VersionMinor);

                if (!standards.Any())
                {
                    logger.LogInformation($"Failed to get standard for {searchResult.StdCode}");
                    continue;
                }

                PopulateMultipleVersionResults(allOptions, searchResult, standards);
            }

            return searchResults;
        }

        private static void PopulateMultipleVersionResults(IEnumerable<StandardOptions> allOptions, SearchResult searchResult, IOrderedEnumerable<Standard> standards)
        {
            var firstStandard = standards.First();
            searchResult.Standard = firstStandard.Title;
            searchResult.Level = firstStandard.Level;
            searchResult.Versions = standards.Select(s => new StandardVersion
            {
                Title = s.Title,
                StandardUId = s.StandardUId,
                Version = s.Version,
                Options = allOptions.SingleOrDefault(o => o.StandardUId == s.StandardUId)?.CourseOption
            }).ToList();
        }

        private static void PopulateSingleVersionResult(Standard learnerStandard, IEnumerable<StandardOptions> allOptions, SearchResult searchResult)
        {
            bool optionValid = false;

            if (!string.IsNullOrWhiteSpace(searchResult.Option))
            {
                var standardOptions = allOptions.SingleOrDefault(o => o.StandardUId == learnerStandard.StandardUId)?.CourseOption;
                if (standardOptions.Contains(searchResult.Option, StringComparer.InvariantCultureIgnoreCase))
                {
                    optionValid = true;
                }
            }

            searchResult.Standard = learnerStandard.Title;
            searchResult.Level = learnerStandard.Level;
            searchResult.Versions = new List<StandardVersion> { new StandardVersion
                    {
                        Title = learnerStandard.Title,
                        StandardUId = learnerStandard.StandardUId,
                        Version = learnerStandard.Version,
                        Options = optionValid ? new List<string>{searchResult.Option} : allOptions.SingleOrDefault(o => o.StandardUId == searchResult.StandardUId)?.CourseOption
                    }};

        }

        public static List<SearchResult> MatchUpExistingCompletedStandards(this List<SearchResult> searchResults, SearchQuery request, string likedSurname, IEnumerable<int> approvedStandards, ICertificateRepository certificateRepository, IContactQueryRepository contactRepository, IOrganisationQueryRepository _organisationRepository, ILogger<SearchHandler> logger)
        {
            logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
            var certificates = certificateRepository.GetDraftAndCompletedCertificatesFor(request.Uln).Result;
            logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");

            // Don't match up existing standards if paramters for filtering not passed in.
            if (string.IsNullOrWhiteSpace(likedSurname) || approvedStandards == null || approvedStandards.Count() == 0)
                return searchResults;

            var searchingEpao = _organisationRepository.Get(request.EpaOrgId).Result;

            if (searchResults.Count > 0)
            {
                foreach (var searchResult in searchResults)
                {
                    var certificate = certificates.SingleOrDefault(s => s.StandardCode == searchResult.StdCode &&
                                                                    s.Status != CertificateStatus.Draft);
                    if (certificate != null)
                    {
                        searchResult.PopulateCertificateBasicInformation(certificate);
                        searchResult.PopulateCertificateExtraInformationDependingOnPermission(request, certificateRepository, contactRepository, certificate, searchingEpao, logger);
                    }
                }
            }
            else if (certificates.Count > 0)
            {
                foreach (var certificate in certificates)
                {
                    // Don't return certficate if the EPAO isn't able to assess that standard
                    if (!approvedStandards.Contains(certificate.StandardCode))
                        continue;

                    var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                    
                    // Don't return certificate if the name does not match.
                    if (!string.Equals(certificateData.LearnerFamilyName.Trim(), likedSurname.Trim(), StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    // Create a new search result as it would be when returned by the Learner record
                    var searchResult = new SearchResult
                    {
                        Uln = certificate.Uln,
                        FamilyName = certificateData.LearnerFamilyName,
                        GivenNames = certificateData.LearnerGivenNames,
                        StdCode = certificate.StandardCode,
                        UkPrn = certificate.ProviderUkPrn,
                        CreatedAt = certificate.CreatedAt,
                        LearnStartDate = certificateData.LearningStartDate
                    };

                    searchResult.PopulateCertificateBasicInformation(certificate);
                    searchResult.PopulateCertificateExtraInformationDependingOnPermission(request, certificateRepository, contactRepository, certificate, searchingEpao, logger);

                    searchResults.Add(searchResult);
                }
            }

            return searchResults;
        }

        public static SearchResult PopulateCertificateBasicInformation(this SearchResult searchResult, Certificate certificate)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            searchResult.CertificateReference = certificate.CertificateReference;
            searchResult.CertificateId = certificate.Id;
            searchResult.CertificateStatus = certificate.Status;
            searchResult.LearnStartDate = certificateData.LearningStartDate;
            
            // If the Certificate was a fail, maintain the original details of the certificate version and option
            // even if Learner has changed under the hood
            // Otherwise prioritise Learner information which takes precedence
            if(certificate.Status == CertificateStatus.Submitted && certificateData.OverallGrade == CertificateGrade.Fail)
            {
                searchResult.Version = certificateData.Version;
                searchResult.Option = certificateData.CourseOption;
            } else
            {
                searchResult.Version = string.IsNullOrWhiteSpace(searchResult.Version) ? certificateData.Version : searchResult.Version;
                searchResult.Option = string.IsNullOrWhiteSpace(searchResult.Option) ? certificateData.CourseOption : searchResult.Option;
            }

            searchResult.IsPrivatelyFunded = certificate.IsPrivatelyFunded;

            return searchResult;
        }

        public static SearchResult PopulateCertificateExtraInformationDependingOnPermission(this SearchResult searchResult,
            SearchQuery request, ICertificateRepository certificateRepository, IContactQueryRepository contactRepository,
            Certificate certificate, Organisation searchingEpao, ILogger<SearchHandler> logger)
        {
            var certificateLogs = certificateRepository.GetCertificateLogsFor(certificate.Id).Result;
            logger.LogInformation($"MatchUpExistingCompletedStandards After GetCertificateLogsFor CertificateId {certificate.Id}");
            var createdLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Draft);

            var submittedLogEntry = certificateLogs.FirstOrDefault(l => l.Action == CertificateActions.Submit);

            if (submittedLogEntry == null)
            {
                return searchResult;
            }

            var submittingContact = contactRepository.GetContact(submittedLogEntry.Username).Result ?? contactRepository.GetContact(certificate.UpdatedBy).Result;
            var createdContact = contactRepository.GetContact(createdLogEntry?.Username).Result ?? contactRepository.GetContact(certificate.CreatedBy).Result;

            var lastUpdatedLogEntry = certificateLogs.Aggregate((i1, i2) => i1.EventTime > i2.EventTime ? i1 : i2) ?? submittedLogEntry;
            var lastUpdatedContact = contactRepository.GetContact(lastUpdatedLogEntry.Username).Result;

            logger.LogInformation($"MatchUpExistingCompletedStandards After GetContact for CertificateId {certificate.Id}");

            var searchingContact = contactRepository.GetContact(request.Username).Result;

            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            if (submittingContact != null && searchingContact != null && submittingContact.OrganisationId == searchingContact.OrganisationId)
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
            else if (certificate.OrganisationId == searchingEpao?.Id)
            {
                searchResult.ShowExtraInfo = true;
                searchResult.OverallGrade = certificateData.OverallGrade;
                searchResult.SubmittedBy = submittedLogEntry.Username ?? certificate.UpdatedBy;
                searchResult.SubmittedAt = submittedLogEntry.EventTime.UtcToTimeZoneTime(); // This needs to be local time 
                searchResult.AchDate = certificateData.AchievementDate;
                searchResult.UpdatedBy = lastUpdatedLogEntry.Username ?? certificate.UpdatedBy;
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

            return searchResult;
        }
    }
}
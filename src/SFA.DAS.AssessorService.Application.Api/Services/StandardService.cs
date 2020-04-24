using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.ExternalApis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class StandardService : IStandardService
    {
        private readonly CacheService _cacheService;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IIfaStandardsApiClient _ifaStandardsApiClient;
        private readonly ILogger<StandardService> _logger;
        private readonly IStandardRepository _standardRepository;

        public StandardService(CacheService cacheService, IAssessmentOrgsApiClient assessmentOrgsApiClient, IIfaStandardsApiClient ifaStandardsApiClient, ILogger<StandardService> logger, IStandardRepository standardRepository)
        {
            _cacheService = cacheService;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _ifaStandardsApiClient = ifaStandardsApiClient;
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<IEnumerable<StandardCollation>> GetAllStandards()
        {
            var results = await _cacheService.RetrieveFromCache<IEnumerable<StandardCollation>>("StandardCollations");

            if (results != null)
                return results;

            var standardCollations = await _standardRepository.GetStandardCollations();

            await _cacheService.SaveToCache("StandardCollations", standardCollations, 8);
            return standardCollations;
        }

        public async Task<StandardCollation> GetStandard(int standardId)
        {
            StandardCollation standardCollation = null;

            try
            {
                standardCollation = await _standardRepository.GetStandardCollationByStandardId(standardId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD COLLATION: Failed to get for standard id: {standardId}");
            }

            return standardCollation;
        }

        public async Task<StandardCollation> GetStandard(string referenceNumber)
        {
            StandardCollation standardCollation = null;

            try
            {
                standardCollation = await _standardRepository.GetStandardCollationByReferenceNumber(referenceNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"STANDARD COLLATION: Failed to get for standard reference: {referenceNumber}");
            }

            return standardCollation;
        }

        public async Task<List<Option>> GetOptions(int stdCode)
        {
            List<Option> options = null;

            try
            {
                options = await _standardRepository.GetOptions(stdCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OPTION: Failed to get for options for stdcode: {stdCode}");
            }

            return options;
        }

        public async Task<IEnumerable<IfaStandard>> GetIfaStandards()
        {
            try
            {
                _logger.LogInformation("STANDARD COLLATION: Starting gathering of all IFA Standard details");
                return await _ifaStandardsApiClient.GetAllStandards();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "STANDARD COLLATION: Failed to gather all IFA Standard details");
            }

            return null;
        }

        public async Task<IEnumerable<StandardCollation>> GatherAllApprovedStandardDetails(List<IfaStandard> approvedIfaStandards)
        {
            _logger.LogInformation("STANDARD COLLATION: Starting gathering of all WIN Standard details");
            
            // get all the standards from the apprenticeship programs api - formally known as WIN aka provider api 
            var winResults = await _assessmentOrgsApiClient.GetAllStandardsV2();

            _logger.LogInformation("STANDARD COLLATION: Start collating approved IFA and WIN standards");
            var collation = CollateWinAndIfaStandardDetails(winResults, approvedIfaStandards);

            _logger.LogInformation($"STANDARD COLLATION: Add unmatched approved IFA Standards to collation");
            AddIfaOnlyStandardsToCollatedStandards(approvedIfaStandards, collation);

            _logger.LogInformation($"STANDARD COLLATION: Approved collation finished");

            return collation;
        }

        public IEnumerable<StandardNonApprovedCollation> GatherAllNonApprovedStandardDetails(List<IfaStandard> nonApprovedIfaStandards)
        {
            _logger.LogInformation("STANDARD COLLATION: Start collating non-approved IFA and WIN standards");
            var collation = CollateNonApprovedIfaStandardDetails(nonApprovedIfaStandards);

            _logger.LogInformation($"STANDARD COLLATION: Non-Approved collation finished");

            return collation;
        }

        public async Task<IEnumerable<EPORegisteredStandards>> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId)
        {
            var results = await _standardRepository.GetEpaoRegisteredStandards(endPointAssessorOrganisationId, short.MaxValue, null);
            return results.PageOfResults;
        }

        private void AddIfaOnlyStandardsToCollatedStandards(List<IfaStandard> ifaResults, List<StandardCollation> collation)
        {
            var uncollatedIfaStandards = ifaResults?.Where(ifaStandard => collation.All(s => s.StandardId != ifaStandard.LarsCode))
                .ToList();

            if (uncollatedIfaStandards != null)
            {
                foreach (var ifaStandard in uncollatedIfaStandards)
                {
                    var standard = MapDataToStandardCollation(ifaStandard.LarsCode, ifaStandard, null);
                    collation.Add(standard);
                }
            }
        }

        private List<StandardCollation> CollateWinAndIfaStandardDetails(List<StandardSummary> winResults, List<IfaStandard> ifaResults)
        {
            var collation = new List<StandardCollation>();
            foreach (var winStandard in winResults)
            {
                if (!int.TryParse(winStandard.Id, out int standardId)) continue;
                var ifaStandardToMatch = ifaResults?.FirstOrDefault(x => x.LarsCode.ToString() == winStandard.Id);
                var standard = MapDataToStandardCollation(standardId, ifaStandardToMatch, winStandard);
                collation.Add(standard);
            }

            return collation;
        }

        private List<StandardNonApprovedCollation> CollateNonApprovedIfaStandardDetails(List<IfaStandard> ifaResultsWithoutLarsCode)
        {
            var collation = new List<StandardNonApprovedCollation>();
            foreach (var ifaStandard in ifaResultsWithoutLarsCode)
            {
                var standard = MapDataToStandardNonApprovedCollation(ifaStandard);
                collation.Add(standard);
            }

            return collation;
        }

        private StandardCollation MapDataToStandardCollation(int standardId, IfaStandard ifaStandard, StandardSummary winStandard)
        {
            return new StandardCollation
            {
                StandardId = standardId,
                ReferenceNumber = ifaStandard?.ReferenceNumber,
                Title = ifaStandard?.Title ?? winStandard?.Title,
                Options = ifaStandard?.GetOptionTitles(),
                StandardData = new StandardData
                {
                    Category = ifaStandard?.Route,
                    IfaStatus = ifaStandard?.Status,
                    EqaProviderName = ifaStandard?.EqaProvider?.ProviderName,
                    EqaProviderContactName = ifaStandard?.EqaProvider?.ContactName,
                    EqaProviderContactAddress = ifaStandard?.EqaProvider?.ContactAddress,
                    EqaProviderContactEmail = ifaStandard?.EqaProvider?.ContactEmail,
                    EqaProviderWebLink = ifaStandard?.EqaProvider?.WebLink,
                    IntegratedDegree = ifaStandard?.IntegratedDegree,
                    EffectiveFrom = winStandard?.EffectiveFrom,
                    EffectiveTo = winStandard?.EffectiveTo,
                    Level = winStandard?.Level ?? ifaStandard?.Level,
                    LastDateForNewStarts = winStandard?.LastDateForNewStarts,
                    IfaOnly = winStandard == null,
                    Duration = winStandard?.Duration ?? ifaStandard?.TypicalDuration,
                    MaxFunding = winStandard?.CurrentFundingCap ?? ifaStandard?.MaxFunding,
                    Trailblazer = ifaStandard?.TbMainContact,
                    PublishedDate = ifaStandard?.ApprovedForDelivery,
                    IsPublished = winStandard?.IsPublished ?? ifaStandard?.IsPublished,
                    Ssa1 = ifaStandard?.Ssa1,
                    Ssa2 = ifaStandard?.Ssa2,
                    OverviewOfRole = ifaStandard?.OverviewOfRole,
                    IsActiveStandardInWin = winStandard?.IsActiveStandard,
                    FatUri = winStandard?.Uri,
                    IfaUri = ifaStandard?.Url,
                    AssessmentPlanUrl = ifaStandard?.AssessmentPlanUrl,
                    StandardPageUrl = ifaStandard?.StandardPageUrl
                }
            };
        }

        private StandardNonApprovedCollation MapDataToStandardNonApprovedCollation(IfaStandard ifaStandard)
        {
            return new StandardNonApprovedCollation
            {
                ReferenceNumber = ifaStandard.ReferenceNumber,
                Title = ifaStandard.Title,
                StandardData = new StandardNonApprovedData
                {
                    Category = ifaStandard.Route,
                    IfaStatus = ifaStandard.Status,
                    IntegratedDegree = ifaStandard.IntegratedDegree,
                    Level = ifaStandard.Level,
                    Duration = ifaStandard.TypicalDuration,
                    Trailblazer = ifaStandard.TbMainContact,
                    OverviewOfRole = ifaStandard.OverviewOfRole,
                    StandardPageUrl = ifaStandard.StandardPageUrl
                }
            };
        }
    }
}

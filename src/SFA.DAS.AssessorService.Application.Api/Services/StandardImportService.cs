using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class StandardImportService : IStandardImportService
    {
        private readonly IStandardRepository standardRepository;

        public StandardImportService(IStandardRepository standardRepository)
        {
            this.standardRepository = standardRepository;
        }

        public async Task DeleteAllStandardsAndOptions()
        {
            await Task.WhenAll(standardRepository.DeleteAllStandards(), standardRepository.DeleteAllOptions());
        }

        public async Task LoadStandards(IEnumerable<StandardDetailResponse> standards)
        {
            Func<StandardDetailResponse, Standard> MapGetStandardsListItemToStandard = source => new Standard
            {
                StandardUId = source.StandardUId,
                IfateReferenceNumber = source.IfateReferenceNumber,
                LarsCode = source.LarsCode,
                Title = source.Title,
                Version = source.Version,
                Level = source.Level,
                Status = source.Status,
                TypicalDuration = source.TypicalDuration,
                MaxFunding = source.MaxFunding,
                IsActive = source.IsActive,
                LastDateStarts = source.StandardDates?.LastDateStarts,
                EffectiveFrom = source.StandardDates?.EffectiveFrom,
                EffectiveTo = source.StandardDates?.EffectiveTo,
                VersionApprovedForDelivery = source.VersionDetail.ApprovedForDelivery,
                VersionEarliestStartDate = source.VersionDetail.EarliestStartDate,
                VersionLatestEndDate = source.VersionDetail.LatestEndDate,
                VersionLatestStartDate = source.VersionDetail.LatestStartDate,
                ProposedMaxFunding = source.VersionDetail.ProposedMaxFunding,
                ProposedTypicalDuration = source.VersionDetail.ProposedTypicalDuration
            };

            await standardRepository.InsertStandards(standards.Select(MapGetStandardsListItemToStandard));
        }

        public async Task LoadOptions(IEnumerable<StandardDetailResponse> standards)
        {
            var standardsWithOptions = standards.Where(s => s.Options != null && s.Options.Any());
            IEnumerable<StandardOption> optionsToInsert = new List<StandardOption>();
            foreach(var standard in standardsWithOptions)
            {
                // Union to ensure no duplicates.
                optionsToInsert = optionsToInsert.Union(standard.Options.Select(s => new StandardOption { StandardUId = standard.StandardUId, OptionName = s }));
            }

            await standardRepository.InsertOptions(optionsToInsert);
        }

        public async Task UpsertStandardCollations(IEnumerable<StandardDetailResponse> standards)
        {
            Func<StandardDetailResponse, StandardCollation> MapGetStandardsListItemToStandardCollation = source => new StandardCollation
            {
                StandardId = source.LarsCode,
                ReferenceNumber = source.IfateReferenceNumber,
                Title = source.Title,
                Options = source.Options,
                StandardData = new StandardData
                {
                    Category = source.Route,
                    IfaStatus = source.Status,
                    EqaProviderName = source.EqaProvider?.Name,
                    EqaProviderContactName = source.EqaProvider?.ContactName,
                    EqaProviderContactEmail = source.EqaProvider?.ContactEmail,
                    EqaProviderWebLink = source.EqaProvider?.WebLink,
                    IntegratedDegree = source.IntegratedDegree,
                    EffectiveFrom = source.StandardDates.EffectiveFrom,
                    EffectiveTo = source.StandardDates.EffectiveTo,
                    Level = source.Level,
                    LastDateForNewStarts = source.StandardDates.LastDateStarts,
                    IfaOnly = source.LarsCode == 0,
                    Duration = source.TypicalDuration,
                    MaxFunding = source.MaxFunding,
                    Trailblazer = source.TrailBlazerContact,
                    PublishedDate = source.VersionDetail.ApprovedForDelivery,
                    IsPublished = source.LarsCode > 0,
                    Ssa2 = source.SectorSubjectAreaTier2Description,
                    OverviewOfRole = source.OverviewOfRole,
                    IsActiveStandardInWin = source.IsActive,
                    AssessmentPlanUrl = source.AssessmentPlanUrl,
                    StandardPageUrl = source.StandardPageUrl
                }
            };

            var standardCollations = standards.Select(MapGetStandardsListItemToStandardCollation).ToList();

            await standardRepository.UpsertApprovedStandards(standardCollations);
        }

        public async Task UpsertStandardNonApprovedCollations(IEnumerable<StandardDetailResponse> standards)
        {
            Func<StandardDetailResponse, StandardNonApprovedCollation> MapGetStandardsListItemToStandardNonApprovedCollation = source => new StandardNonApprovedCollation
            {
                ReferenceNumber = source.IfateReferenceNumber,
                Title = source.Title,
                StandardData = new StandardNonApprovedData
                {
                    Level = source.Level,
                    Category = source.Route,
                    IfaStatus = source.Status,
                    IntegratedDegree = source.IntegratedDegree,
                    Duration = source.TypicalDuration,
                    MaxFunding = source.MaxFunding,
                    Trailblazer = source.TrailBlazerContact,
                    OverviewOfRole = source.OverviewOfRole,
                    StandardPageUrl = source.StandardPageUrl
                }
            };

            var standardNonApprovedCollations = standards.Select(MapGetStandardsListItemToStandardNonApprovedCollation).ToList();

            await standardRepository.UpsertNonApprovedStandards(standardNonApprovedCollations);
        }
    }
}

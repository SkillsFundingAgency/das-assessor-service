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
        public async Task LoadStandards(IEnumerable<GetStandardByIdResponse> standards)
        {
            Func<GetStandardByIdResponse, Standard> MapGetStandardsListItemToStandard = source => new Standard
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

            await standardRepository.DeleteAll();

            var tasks = standards.Select(MapGetStandardsListItemToStandard).Select(standardRepository.Insert);

            await Task.WhenAll(tasks);
        }

        public async Task UpsertStandardCollations(IEnumerable<GetStandardByIdResponse> standards)
        {
            Func<GetStandardByIdResponse, StandardCollation> MapGetStandardsListItemToStandard = source => new StandardCollation
            {
                StandardId = source.LarsCode,
                ReferenceNumber = source.IfateReferenceNumber,
                Title = source.Title,
                Options = source.Options,
                StandardData = new StandardData
                {
                    Category = source.Route,
                    IfaStatus = source.Status,
                    //EqaProviderName = source.EqaProvider?.ProviderName,
                    EqaProviderContactName = source.EqaProvider?.ContactName,
                    //EqaProviderContactAddress = source.EqaProvider?.ContactAddress,
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
                    //Ssa1 = source.ssa1,
                    Ssa2 = source.SectorSubjectAreaTier2Description,
                    OverviewOfRole = source.OverviewOfRole,
                    IsActiveStandardInWin = source.IsActive,
                    FatUri = "",
                    //IfaUri = source.Url,
                    AssessmentPlanUrl = source.AssessmentPlanUrl,
                    StandardPageUrl = source.StandardPageUrl
                }
            };

            var standardCollations = standards.Select(MapGetStandardsListItemToStandard).ToList();

            await standardRepository.UpsertApprovedStandards(standardCollations);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class StandardImportService : IStandardImportService
    {
        private readonly IStandardRepository standardRepository;

        public StandardImportService(IStandardRepository standardRepository)
        {
            this.standardRepository = standardRepository;
        }

        public async Task PrepareImport()
        {
            await standardRepository.PrepareStandardsImport();
        }

        public async Task StageStandards(IEnumerable<StandardDetailResponse> standards)
        {
            Func<StandardDetailResponse, Standard> MapGetStandardsListItemToStandard = source => new Standard
            {
                StandardUId = source.StandardUId,
                IfateReferenceNumber = source.IfateReferenceNumber,
                LarsCode = source.LarsCode,
                Title = source.Title,
                CoronationEmblem = source.CoronationEmblem,
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
                ProposedTypicalDuration = source.VersionDetail.ProposedTypicalDuration,
                EPAChanged = source.EPAChanged,
                StandardPageUrl = source.StandardPageUrl,
                TrailBlazerContact = source.TrailBlazerContact,
                Route = source.Route,
                VersionMajor = source.VersionMajor,
                VersionMinor = source.VersionMinor,
                IntegratedDegree = source.IntegratedDegree,
                EqaProviderName = source.EqaProvider?.Name,
                EqaProviderContactName = source.EqaProvider?.ContactName,
                EqaProviderContactEmail = source.EqaProvider?.ContactEmail,
                OverviewOfRole = source.OverviewOfRole,
                EpaoMustBeApprovedByRegulatorBody = source.EpaoMustBeApprovedByRegulatorBody,
            };

            await standardRepository.InsertStandardsIntoStaging(standards.Select(MapGetStandardsListItemToStandard));
        }

        public async Task StageOptions(IEnumerable<StandardDetailResponse> standards)
        {
            var optionsToInsert = standards.SelectMany(standard =>
                (standard.Options ?? new List<string>()).Select(option => new StandardOption
                {
                    StandardUId = standard.StandardUId,
                    OptionName = option
                }));

            await standardRepository.InsertOptionsIntoStaging(optionsToInsert);
        }

        public async Task MergeStandardsFromStaging()
        {
            await standardRepository.MergeStandardsFromStaging();
        }
    }
}

using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}

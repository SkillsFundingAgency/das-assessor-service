using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories
{
    public class FixtureBase : IDisposable
    {
        private readonly List<OrganisationModel> _organisations = new List<OrganisationModel>();
        private readonly List<StandardModel> _standards = new List<StandardModel>();
        private readonly List<OrganisationStandardModel> _organisationStandards = new List<OrganisationStandardModel>();
        private readonly List<OrganisationStandardVersionModel> _organisationStandardVersions = new List<OrganisationStandardVersionModel>();

        public FixtureBase() 
        {
            // this is to workaround the other tests which are not clearing up after themselves properly
            DeleteAllRecords();
        }

        public void AddOrganisation(string endPointAssessorName, string endPointAssessorOrganisationId, int ukprn)
        {
            var organisation = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                EndPointAssessorName = endPointAssessorName,
                EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                EndPointAssessorUkprn = ukprn,
                PrimaryContact = null,
                OrganisationTypeId = 1,
                OrganisationData = null,
                Status = OrganisationStatus.Live
            };

            _organisations.Add(organisation);
            OrganisationHandler.InsertRecord(organisation);
        }

        public void AddStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveTo)
        {
            var standard = new StandardModel
            {
                StandardUId = $"{referenceNumber}_{version}",
                IFateReferenceNumber = referenceNumber,
                LarsCode = larsCode,
                Title = title,
                Version = version,
                Level = 4,
                Status = "Approved for delivery",
                EffectiveFrom = DateTime.Now.AddDays(-50),
                EffectiveTo = effectiveTo,
                TypicalDuration = 12,
                VersionApprovedForDelivery = DateTime.Now.AddDays(-50),
                TrailblazerContact = "TrailblazerContact",
                StandardPageUrl = "www.standard.com",
                OverviewOfRole = "OverviewOfRole"
            };

            _standards.Add(standard);
            StandardsHandler.InsertRecord(standard);
        }

        public void AddOrganisationStandard(int id, string endPointAssessorOrganisationId, int larsCode, string standardReference, 
            DateTime? effectiveFrom, DateTime? effectiveTo, DateTime? dateStandardApprovedOnRegister)
        {
            var organisationStandard = new OrganisationStandardModel
            {
                Id = id,
                EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                StandardCode = larsCode,
                Status = OrganisationStatus.Live,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                DateStandardApprovedOnRegister = dateStandardApprovedOnRegister,
                Comments = string.Empty,
                StandardReference = standardReference,
            };

            _organisationStandards.Add(organisationStandard);
            OrganisationStandardHandler.InsertRecord(organisationStandard);
        }

        public void AddOrganisationStandardVersion(string endPointAssessorOrganisationId, int larsCode, string standardReference, string version, 
            DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            var organisationStandard = _organisationStandards.First(p => p.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && p.StandardCode == larsCode);
            var standard = _standards.First(p => p.IFateReferenceNumber == standardReference && p.Version == version);

            var organisationStandardVersion = new OrganisationStandardVersionModel
            {
                OrganisationStandardId = organisationStandard.Id,
                StandardUId = standard.StandardUId,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                Version = standard.Version,
                DateVersionApproved = DateTime.Now.AddMonths(-12),
                Status = OrganisationStatus.Live
            };

            _organisationStandardVersions.Add(organisationStandardVersion);
            OrganisationStandardVersionHandler.InsertRecord(organisationStandardVersion);
        }

        public void Dispose()
        {
            DeleteAllRecords();
        }

        protected static void DeleteAllRecords()
        {
            OrganisationStandardVersionHandler.DeleteAllRecords();
            OrganisationStandardHandler.DeleteAllRecords();
            OrganisationHandler.DeleteAllRecords();
            StandardsHandler.DeleteAllRecords();
        }
    }
}

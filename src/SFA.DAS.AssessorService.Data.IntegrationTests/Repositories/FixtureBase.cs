using FluentAssertions;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories
{
    public class FixtureBase<T> where T : class, IDisposable
    {
        private readonly List<OrganisationModel> _organisations = new List<OrganisationModel>();
        private readonly List<ContactModel> _contacts = new List<ContactModel>();
        private readonly List<StandardModel> _standards = new List<StandardModel>();
        private readonly List<OrganisationStandardModel> _organisationStandards = new List<OrganisationStandardModel>();
        private readonly List<OrganisationStandardVersionModel> _organisationStandardVersions = new List<OrganisationStandardVersionModel>();

        private readonly List<StagingOfqualOrganisationModel> _stagingOfqualOrganisations = new List<StagingOfqualOrganisationModel>();
        private readonly List<OfqualOrganisationModel> _ofqualOrganisations = new List<OfqualOrganisationModel>();

        private readonly List<StagingOfqualStandardModel> _stagingOfqualStandards = new List<StagingOfqualStandardModel>();
        private readonly List<OfqualStandardModel> _ofqualStandards = new List<OfqualStandardModel>();

        public FixtureBase()
        {
            // this is to workaround the other tests which are not clearing up after themselves properly
            DeleteAllRecords();
        }

        public T WithOrganisation(string endPointAssessorName, string endPointAssessorOrganisationId, int ukprn, string recognitionNumber)
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
                Status = OrganisationStatus.Live,
                RecognitionNumber = recognitionNumber
            };

            _organisations.Add(organisation);
            OrganisationHandler.InsertRecord(organisation);

            return this as T;
        }

        public T WithContact(string displayName, string email, string endPointAssessorOrganisationId, string username)
        {
            var organisation = _organisations.First(p => p.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);

            var contact = ContactsHandler.Create(
                Guid.NewGuid(),
                DateTime.Now,
                displayName,
                email,
                endPointAssessorOrganisationId,
                organisation.Id,
                "Live",
                username);

            _contacts.Add(contact);
            ContactsHandler.InsertRecord(contact);

            return this as T;
        }

        public T WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveTo, string eqaProviderName = "")
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
                EqaProviderName = eqaProviderName,
                OverviewOfRole = "OverviewOfRole",
            };

            _standards.Add(standard);
            StandardsHandler.InsertRecord(standard);

            return this as T;
        }

        public T WithOrganisationStandard(int id, string endPointAssessorOrganisationId, int larsCode, string standardReference,
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

            return this as T;
        }

        public T WithOrganisationStandardVersion(string endPointAssessorOrganisationId, int larsCode, string standardReference, string version,
            DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            var organisationStandard = _organisationStandards.First(p => p.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && p.StandardCode == larsCode);
            var standard = _standards.First(p => p.IFateReferenceNumber == standardReference && p.Version == version);

            var organisationStandardVersion = new OrganisationStandardVersionModel
            {
                OrganisationStandardId = organisationStandard.Id.Value,
                StandardUId = standard.StandardUId,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                Version = standard.Version,
                DateVersionApproved = DateTime.Now.AddMonths(-12),
                Status = OrganisationStatus.Live
            };

            _organisationStandardVersions.Add(organisationStandardVersion);
            OrganisationStandardVersionHandler.InsertRecord(organisationStandardVersion);

            return this as T;
        }

        public T WithStagingOfqualOrganisation(
                string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
                string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
                string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime? ofqualRecognisedTo)
        {
            var stagingOfqualOrganisation = new StagingOfqualOrganisationModel
            {
                RecognitionNumber = recognitionNumber,
                Name = name,
                LegalName = legalName,
                Acronym = acronym,
                Email = email,
                Website = website,
                HeadOfficeAddressLine1 = headOfficeAddressLine1,
                HeadOfficeAddressLine2 = headOfficeAddressLine2,
                HeadOfficeAddressTown = headOfficeAddressTown,
                HeadOfficeAddressCounty = headOfficeAddressCounty,
                HeadOfficeAddressPostcode = headOfficeAddressPostcode,
                HeadOfficeAddressCountry = headOfficeAddressCountry,
                HeadOfficeAddressTelephone = headOfficeAddressTelephone,
                OfqualStatus = ofqualStatus,
                OfqualRecognisedFrom = ofqualRecognisedFrom,
                OfqualRecognisedTo = ofqualRecognisedTo
            };

            _stagingOfqualOrganisations.Add(stagingOfqualOrganisation);
            StagingOfqualOrganisationHandler.InsertRecord(stagingOfqualOrganisation);

            return this as T;
        }

        public T WithOfqualOrganisation(
            Guid id, string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime? ofqualRecognisedTo,
            DateTime createdAt, DateTime? updatedAt)
        {
            var ofqualOrganisation = new OfqualOrganisationModel
            {
                Id = id,
                RecognitionNumber = recognitionNumber,
                Name = name,
                LegalName = legalName,
                Acronym = acronym,
                Email = email,
                Website = website,
                HeadOfficeAddressLine1 = headOfficeAddressLine1,
                HeadOfficeAddressLine2 = headOfficeAddressLine2,
                HeadOfficeAddressTown = headOfficeAddressTown,
                HeadOfficeAddressCounty = headOfficeAddressCounty,
                HeadOfficeAddressPostcode = headOfficeAddressPostcode,
                HeadOfficeAddressCountry = headOfficeAddressCountry,
                HeadOfficeAddressTelephone = headOfficeAddressTelephone,
                OfqualStatus = ofqualStatus,
                OfqualRecognisedFrom = ofqualRecognisedFrom,
                OfqualRecognisedTo = ofqualRecognisedTo,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            _ofqualOrganisations.Add(ofqualOrganisation);
            OfqualOrganisationHandler.InsertRecord(ofqualOrganisation);

            return this as T;
        }

        public T WithStagingOfqualStandard(
                string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var stagingOfqualStandard = new StagingOfqualStandardModel
            {
                RecognitionNumber = recognitionNumber,
                OperationalStartDate = operationalStartDate,
                OperationalEndDate = operationalEndDate,
                IfateReferenceNumber = ifateReferenceNumber
            };

            _stagingOfqualStandards.Add(stagingOfqualStandard);
            StagingOfqualStandardHandler.InsertRecord(stagingOfqualStandard);

            return this as T;
        }

        public T WithOfqualStandard(
            Guid id, string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber,
            DateTime createdAt, DateTime? updatedAt)
        {
            var stagingOfqualStandard = new OfqualStandardModel
            {
                Id = id,
                RecognitionNumber = recognitionNumber,
                OperationalStartDate = operationalStartDate,
                OperationalEndDate = operationalEndDate,
                IfateReferenceNumber = ifateReferenceNumber,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            _ofqualStandards.Add(stagingOfqualStandard);
            OfqualStandardHandler.InsertRecord(stagingOfqualStandard);

            return this as T;
        }

        public async Task<T> VerifyOrganisationStandardExists(OrganisationStandardModel organisationStandard)
        {
            var result = await OrganisationStandardHandler.QueryFirstOrDefaultAsync(organisationStandard);
            result.Should().NotBeNull();

            return this as T;
        }

        public void Dispose()
        {
            DeleteAllRecords();
        }

        protected static void DeleteAllRecords()
        {
            OfqualOrganisationHandler.DeleteAllRecords();
            OfqualStandardHandler.DeleteAllRecords();
            OrganisationStandardVersionHandler.DeleteAllRecords();
            OrganisationStandardHandler.DeleteAllRecords();
            OrganisationHandler.DeleteAllRecords();
            StandardsHandler.DeleteAllRecords();
            StagingOfqualOrganisationHandler.DeleteAllRecords();
            StagingOfqualStandardHandler.DeleteAllRecords();
        }
    }
}
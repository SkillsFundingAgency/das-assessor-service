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
        private readonly List<ApprovalsExtractModel> _approvalsExtracts = new List<ApprovalsExtractModel>();
        private readonly List<IlrModel> _ilrs = new List<IlrModel>();
        private readonly List<OrganisationModel> _organisations = new List<OrganisationModel>();
        private readonly List<ProviderModel> _providers = new List<ProviderModel>();
        private readonly List<ContactModel> _contacts = new List<ContactModel>();
        private readonly List<StandardModel> _standards = new List<StandardModel>();
        private readonly List<OrganisationStandardModel> _organisationStandards = new List<OrganisationStandardModel>();
        private readonly List<OrganisationStandardVersionModel> _organisationStandardVersions = new List<OrganisationStandardVersionModel>();

        private readonly List<StagingOfqualOrganisationModel> _stagingOfqualOrganisations = new List<StagingOfqualOrganisationModel>();
        private readonly List<OfqualOrganisationModel> _ofqualOrganisations = new List<OfqualOrganisationModel>();

        private readonly List<StagingOfqualStandardModel> _stagingOfqualStandards = new List<StagingOfqualStandardModel>();
        private readonly List<OfqualStandardModel> _ofqualStandards = new List<OfqualStandardModel>();

        private readonly List<StagingOfsOrganisationModel> _stagingOfsOrganisations = new List<StagingOfsOrganisationModel>();
        private readonly List<OfsOrganisationModel> _ofsOrganisations = new List<OfsOrganisationModel>();

        public FixtureBase()
        {
            // this is to workaround the other tests which are not clearing up after themselves properly
            DeleteAllRecords();
        }

        public T WithApprovalsExtract(int apprenticeshipId, string firstName, string lastName, string uln, int trainingCode, string trainingCourseVersion, bool trainingCourseVersionConfirmed,
            string trainingCourseOption, string standardUId, DateTime? startDate, DateTime? endDate, DateTime? createdOn, DateTime? updatedOn, DateTime? stopDate, DateTime? pauseDate, DateTime? completionDate,
            int ukprn, string learnRefNumber, int paymentStatus, long employerAccountId, string employerName)
        {
            var approvalsExtract = new ApprovalsExtractModel
            {
                ApprenticeshipId = apprenticeshipId,
                FirstName = firstName,
                LastName = lastName,
                ULN = uln,
                TrainingCode = trainingCode,
                TrainingCourseVersion = trainingCourseVersion,
                TrainingCourseVersionConfirmed = trainingCourseVersionConfirmed,
                TrainingCourseOption = trainingCourseOption,
                StandardUId = standardUId,
                StartDate = startDate,
                EndDate = endDate,
                CreatedOn = createdOn,
                UpdatedOn = updatedOn,
                StopDate = stopDate,
                PauseDate = pauseDate,
                CompletionDate = completionDate,
                UKPRN = ukprn,
                LearnRefNumber = learnRefNumber,
                PaymentStatus = paymentStatus,
                EmployerAccountId = employerAccountId,
                EmployerName = employerName,
            };

            _approvalsExtracts.Add(approvalsExtract);
            ApprovalsExtractHandler.InsertRecord(approvalsExtract);

            return this as T;
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
        
        public T WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveFrom,
            DateTime? effectiveTo, DateTime? versionEarliestStartDate, DateTime? versionLatestStartDate, DateTime? versionLatestEndDate,
            DateTime? versionApprovedForDelivery, bool epaChanged, string eqaProviderName, bool epaoMustBeApprovedByRegulatorBody)
        {
            var standard = StandardsHandler.Create(
                title,
                referenceNumber,
                larsCode,
                version,
                effectiveFrom,
                effectiveTo,
                versionEarliestStartDate,
                versionLatestStartDate,
                versionLatestEndDate,
                versionApprovedForDelivery,
                epaChanged,
                eqaProviderName,
                epaoMustBeApprovedByRegulatorBody);

            _standards.Add(standard);
            StandardsHandler.InsertRecord(standard);

            return this as T;
        }

        public T WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveFrom,
            DateTime? effectiveTo, DateTime? versionEarliestStartDate, DateTime? versionLatestStartDate, DateTime? versionLatestEndDate)
        {
            return WithStandard(title, referenceNumber, larsCode, version, effectiveFrom, effectiveTo,
                versionEarliestStartDate, versionLatestStartDate, versionLatestEndDate, effectiveFrom.GetValueOrDefault(DateTime.Now.Date),
                false, string.Empty, false);
        }

        public T WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveFrom,
            DateTime? effectiveTo, DateTime? versionEarliestStartDate, DateTime? versionLatestStartDate, DateTime? versionLatestEndDate, 
            DateTime? versionApprovedForDelivery)
        {
            return WithStandard(title, referenceNumber, larsCode, version, effectiveFrom, effectiveTo,
                versionEarliestStartDate, versionLatestStartDate, versionLatestEndDate, versionApprovedForDelivery, 
                false, string.Empty, false);
        }

        public T WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveFrom,
            DateTime? effectiveTo, bool epaChanged, string eqaProviderName, bool epaoMustBeApprovedByRegulatorBody)
        {
            return WithStandard(title, referenceNumber, larsCode, version, effectiveFrom, effectiveTo,
                null, null, null, effectiveFrom.GetValueOrDefault(DateTime.Now.Date),
                epaChanged, eqaProviderName, epaoMustBeApprovedByRegulatorBody);
        }

        public T WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveFrom,
            DateTime? effectiveTo, bool epaChanged, string eqaProviderName)
        {
            return WithStandard(title, referenceNumber, larsCode, version, effectiveFrom, effectiveTo,
                null, null, null, effectiveFrom.GetValueOrDefault(DateTime.Now.Date), 
                epaChanged, eqaProviderName, false);
        }

        public T WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveFrom, 
            DateTime? effectiveTo)
        {
            return WithStandard(title, referenceNumber, larsCode, version, effectiveFrom, effectiveTo,
                null, null, null, effectiveFrom.GetValueOrDefault(DateTime.Now.Date), 
                false, string.Empty, false);
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

        public async Task<T> VerifyOfqualOrganisationExists(OfqualOrganisationModel ofqualOrganisation)
        {
            var result = await OfqualOrganisationHandler.QueryFirstOrDefaultAsync(ofqualOrganisation);
            result.Should().NotBeNull();

            return this as T;
        }

        public async Task<T> VerifyOfqualStandardExists(OfqualStandardModel ofqualStandard)
        {
            var result = await OfqualStandardHandler.QueryFirstOrDefaultAsync(ofqualStandard);
            result.Should().NotBeNull();

            return this as T;
        }

        public T WithIlr(
            Guid id, long uln, string givenNames, string familyName, int ukprn, int stdCode, DateTime? learnStartDate, string source, DateTime? createdOn, int completionStatus, DateTime? plannedEndDate)
        {
            var ilr = IlrHandler.Create(id, uln, givenNames, familyName, ukprn, stdCode, learnStartDate, null, source, createdOn, completionStatus, plannedEndDate);
            _ilrs.Add(ilr);
            IlrHandler.InsertRecord(ilr);

            return this as T;
        }

        public T WithIlr(
            Guid id, long uln, int ukprn, int stdCode, string source, DateTime? createdOn, int completionStatus)
        {
            return WithIlr(id, uln, null, null, ukprn, stdCode, null, source, createdOn, completionStatus, null);
        }

        public T WithOfsOrganisation(
            Guid id, int ukprn, DateTime createdAt)
        {
            var ofsOrganisation = new OfsOrganisationModel
            {
                Id = id,
                Ukprn = ukprn,
                CreatedAt = createdAt
            };

            _ofsOrganisations.Add(ofsOrganisation);
            OfsOrganisationHandler.InsertRecord(ofsOrganisation);

            return this as T;
        }

        public T WithProvider(
            int ukprn, string name = "", DateTime? updatedOn = null)
        {
            var provider = new ProviderModel
            {
                Ukprn = ukprn,
                Name = name,
                UpdatedOn = updatedOn ?? DateTime.UtcNow
            };

            _providers.Add(provider);
            ProviderHandler.InsertRecord(provider);

            return this as T;
        }

        public T WithStagingOfsOrganisation(
            int ukprn, string registrationStatus, string highestLevelOfDegreeAwardingPowers)
        {
            var stagingOfsOrganisation = new StagingOfsOrganisationModel
            {
                Ukprn = ukprn,
                RegistrationStatus = registrationStatus,
                HighestLevelOfDegreeAwardingPowers = highestLevelOfDegreeAwardingPowers
            };

            _stagingOfsOrganisations.Add(stagingOfsOrganisation);
            StagingOfsOrganisationHandler.InsertRecord(stagingOfsOrganisation);

            return this as T;
        }

        public async Task<T> VerifyOfsOrganisationRowCount(int count)
        {
            var result = await OfsOrganisationHandler.QueryCountAllAsync();
            result.Should().Be(count);

            return this as T;
        }

        public async Task<T> VerifyOfsOrganisationExists(OfsOrganisationModel ofsOrganisation)
        {
            var result = await OfsOrganisationHandler.QueryFirstOrDefaultAsync(ofsOrganisation);
            result.Should().NotBeNull();

            return this as T;
        }

        public async Task<T> VerifyOrganisationStandardExists(OrganisationStandardModel organisationStandard)
        {
            var result = await OrganisationStandardHandler.QueryFirstOrDefaultAsync(organisationStandard);
            result.Should().NotBeNull();

            return this as T;
        }

        public async Task<T> VerifyOrganisationStandardNotExists(OrganisationStandardModel organisationStandard)
        {
            var result = await OrganisationStandardHandler.QueryFirstOrDefaultAsync(organisationStandard);
            result.Should().BeNull();

            return this as T;
        }

        public async Task<T> VerifyOrganisationStandardRowCount(int count)
        {
            var result = await OrganisationStandardHandler.QueryCountAllAsync();
            result.Should().Be(count);

            return this as T;
        }

        public async Task<T> VerifyOrganisationStandardVersionExists(OrganisationStandardVersionModel organisationStandardVersion)
        {
            var result = await OrganisationStandardVersionHandler.QueryFirstOrDefaultAsync(organisationStandardVersion);
            result.Should().NotBeNull();

            return this as T;
        }

        public async Task<T> VerifyOrganisationStandardVersionRowCount(int count)
        {
            var result = await OrganisationStandardVersionHandler.QueryCountAllAsync();
            result.Should().Be(count);

            return this as T;
        }

        public async Task<T> VerifyOrganisationStandardDeliveryAreaRowCount(int count)
        {
            var result = await OrganisationStandardDeliveryAreaHandler.QueryCountAllAsync();
            result.Should().Be(count);

            return this as T;
        }

        public async Task<T> VerifyLearnerRowCount(int count)
        {
            var result = await LearnerHandler.QueryCountAllAsync();
            result.Should().Be(count);

            return this as T;
        }

        public async Task<T> VerifyLearnerExists(LearnerModel learner)
        {
            var result = await LearnerHandler.QueryFirstOrDefaultAsync(learner);
            result.Should().NotBeNull();

            return this as T;
        }

        public void Dispose()
        {
            DeleteAllRecords();
        }

        protected static void DeleteAllRecords()
        {
            ApprovalsExtractHandler.DeleteAllRecords();
            IlrHandler.DeleteAllRecords();
            LearnerHandler.DeleteAllRecords();
            OfqualOrganisationHandler.DeleteAllRecords();
            OfqualStandardHandler.DeleteAllRecords();
            OfsOrganisationHandler.DeleteAllRecords();
            OrganisationStandardDeliveryAreaHandler.DeleteAllRecords();
            OrganisationStandardVersionHandler.DeleteAllRecords();
            OrganisationStandardHandler.DeleteAllRecords();
            ContactsHandler.DeleteAllRecords();
            OrganisationHandler.DeleteAllRecords();
            ProviderHandler.DeleteAllRecords();
            StandardsHandler.DeleteAllRecords();
            StagingOfqualOrganisationHandler.DeleteAllRecords();
            StagingOfqualStandardHandler.DeleteAllRecords();
            StagingOfsOrganisationHandler.DeleteAllRecords();
        }
    }
}
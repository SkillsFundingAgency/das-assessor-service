using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories
{
    public class LoadOfqalStandardsTests : TestBase
    {
        [TestCase("RN0001", "Name_A", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2", 
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone", 
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName_A", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym_A", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email_A", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website_A", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1_A", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2_A",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown_A", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty_A", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode_A", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry_A", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone_A",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus_A", "2020-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2021-01-01", "2020-02-01")]
        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2021-02-01")]
        public async Task LoadOfqalStandards_UpdateOfqualOrganiation_WhenStagingContainsUpdatedData(
            string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime ofqualRecognisedTo)
        {
            var ofqualOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithOfqualOrganisation(ofqualOrganisationId, "RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
                    "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
                    "OfqualStatus", new DateTime(2020, 1, 1), new DateTime(2020, 1, 2), currentDateTime.AddDays(-1), null)
                .WithStagingOfqualOrganisation(recognitionNumber, name, legalName, acronym, email, website, headOfficeAddressLine1,
                    headOfficeAddressLine2, headOfficeAddressTown, headOfficeAddressCounty, headOfficeAddressPostcode,
                    headOfficeAddressCountry, headOfficeAddressTelephone, ofqualStatus, ofqualRecognisedFrom, ofqualRecognisedTo))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                await results.VerifyOfqualOrganisationExists(OfqualOrganisationHandler.Create(ofqualOrganisationId, recognitionNumber, name, legalName, acronym, email, website, headOfficeAddressLine1,
                    headOfficeAddressLine2, headOfficeAddressTown, headOfficeAddressCounty, headOfficeAddressPostcode,
                    headOfficeAddressCountry, headOfficeAddressTelephone, ofqualStatus, ofqualRecognisedFrom, ofqualRecognisedTo, currentDateTime.AddDays(-1), currentDateTime));
            }
        }

        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2", 
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone", 
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        public async Task LoadOfqalStandards_AddOfqualOrganiation_WhenStagingContainsAddedData(
            string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime ofqualRecognisedTo)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithStagingOfqualOrganisation(recognitionNumber, name, legalName, acronym, email, website, headOfficeAddressLine1,
                    headOfficeAddressLine2, headOfficeAddressTown, headOfficeAddressCounty, headOfficeAddressPostcode,
                    headOfficeAddressCountry, headOfficeAddressTelephone, ofqualStatus, ofqualRecognisedFrom, ofqualRecognisedTo))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                await results.VerifyOfqualOrganisationExists(OfqualOrganisationHandler.Create(null, recognitionNumber, name, legalName, acronym, email, website, headOfficeAddressLine1,
                    headOfficeAddressLine2, headOfficeAddressTown, headOfficeAddressCounty, headOfficeAddressPostcode,
                    headOfficeAddressCountry, headOfficeAddressTelephone, ofqualStatus, ofqualRecognisedFrom, ofqualRecognisedTo, currentDateTime, null));
            }
        }

        [TestCase("RN0001", "Name", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
            "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
            "OfqualStatus", "2020-01-01", "2020-02-01")]
        public async Task LoadOfqalStandards_DoesNotRemoveOfqualOrganiation_WhenStagingContainsRemovedData(
            string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime ofqualRecognisedTo)
        {
            var ofqualOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithOfqualOrganisation(ofqualOrganisationId, recognitionNumber, name, legalName, acronym, email, website, headOfficeAddressLine1,
                    headOfficeAddressLine2, headOfficeAddressTown, headOfficeAddressCounty, headOfficeAddressPostcode,
                    headOfficeAddressCountry, headOfficeAddressTelephone, ofqualStatus, ofqualRecognisedFrom, ofqualRecognisedTo, currentDateTime.AddDays(-1), null))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                await results.VerifyOfqualOrganisationExists(OfqualOrganisationHandler.Create(ofqualOrganisationId, recognitionNumber, name, legalName, acronym, email, website, headOfficeAddressLine1,
                    headOfficeAddressLine2, headOfficeAddressTown, headOfficeAddressCounty, headOfficeAddressPostcode,
                    headOfficeAddressCountry, headOfficeAddressTelephone, ofqualStatus, ofqualRecognisedFrom, ofqualRecognisedTo, currentDateTime.AddDays(-1), null));
            }
        }

        [TestCase("RN0001", "2020-12-01", "2021-02-01", "ST0001")]
        [TestCase("RN0001", "2020-01-01", "2021-02-01", "ST0001")]
        public async Task LoadOfqalStandards_UpdateOfqualStandards_WhenStagingContainsUpdatedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithOfqualStandard(ofqualStandardId, "RN0001", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0001", currentDateTime.AddDays(-1), null)
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                await results.VerifyOfqualStandardExists(OfqualStandardHandler.Create(ofqualStandardId, recognitionNumber, operationalStartDate, operationalEndDate, 
                    ifateReferenceNumber, currentDateTime.AddDays(-1), currentDateTime));
            }
        }

        [TestCase("RN0002", "2020-01-01", "2021-02-01", "ST0001")]
        [TestCase("RN0001", "2020-01-01", "2021-02-01", "ST0002")]
        public async Task LoadOfqalStandards_AddOfqualStandards_WhenStagingContainsAddedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithOfqualStandard(ofqualStandardId, "RN0001", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0001", currentDateTime.AddDays(-1), null)
                .WithStagingOfqualStandard("RN0001", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0001")
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                await results.VerifyOfqualStandardExists(OfqualStandardHandler.Create(ofqualStandardId, "RN0001", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0001", 
                    currentDateTime.AddDays(-1), null));

                await results.VerifyOfqualStandardExists(OfqualStandardHandler.Create(null, recognitionNumber, operationalStartDate, operationalEndDate,
                    ifateReferenceNumber, currentDateTime, null));
            }
        }

        [TestCase("RN0002", "2020-01-01", "2020-02-01", "ST0001")]
        [TestCase("RN0001", "2020-01-01", "2020-02-01", "ST0002")]
        public async Task LoadOfqalStandards_DoesNotRemoveOfqualStandards_WhenStagingContainsRemovedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardIdOne = Guid.NewGuid();
            var ofqualStandardIdTwo = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithOfqualStandard(ofqualStandardIdOne, "RN0001", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0002", currentDateTime.AddDays(-1), null)
                .WithOfqualStandard(ofqualStandardIdTwo, "RN0002", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0001", currentDateTime.AddDays(-1), null)
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                await results.VerifyOfqualStandardExists(OfqualStandardHandler.Create(ofqualStandardIdOne, "RN0001", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0002",
                    currentDateTime.AddDays(-1), null));

                await results.VerifyOfqualStandardExists(OfqualStandardHandler.Create(ofqualStandardIdTwo, "RN0002", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1), "ST0001",
                    currentDateTime.AddDays(-1), null));
            }
        }

        [TestCase("RN0001", "2020-01-01", 0, "2021-02-01", "ST0001")]
        [TestCase("RN0001", "2020-01-01", 5, "2021-02-01", "ST0001")]
        [TestCase("RN0001", "2020-01-01", -5, "2021-02-01", "ST0001")]
        public async Task LoadOfqalStandards_UpdateOfqualStandardsTakesEarlierOperationalStartDate_WhenStagingContainsMultipleUpdatedDataForSameStandard(
            string recognitionNumber, DateTime operationalStartDate, int operationalStartDateOffsetDays, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithOfqualStandard(ofqualStandardId, recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber, currentDateTime.AddDays(-1), null)
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber)
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate.AddDays(operationalStartDateOffsetDays), operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                var expectedOperationalStartDate = operationalStartDate <= operationalStartDate.AddDays(operationalStartDateOffsetDays)
                    ? operationalStartDate
                    : operationalStartDate.AddDays(operationalStartDateOffsetDays);

                // if the operational start date is not changing then no update will be made
                var expectedUpdatedAt = operationalStartDate <= operationalStartDate.AddDays(operationalStartDateOffsetDays)
                    ? (DateTime?)null
                    : currentDateTime;

                await results.VerifyOfqualStandardExists(OfqualStandardHandler.Create(ofqualStandardId, recognitionNumber, expectedOperationalStartDate, operationalEndDate,
                    ifateReferenceNumber, currentDateTime.AddDays(-1), expectedUpdatedAt));
            }
        }

        [TestCase("RN0001", "2020-01-01", "2021-02-01", 0, "ST0001")]
        [TestCase("RN0001", "2020-01-01", "2021-02-01", 5, "ST0001")]
        [TestCase("RN0001", "2020-01-01", "2021-02-01", -5, "ST0001")]
        [TestCase("RN0001", "2020-01-01", null, 0, "ST0001")]
        public async Task LoadOfqalStandards_UpdateOfqualStandardsTakesOperationalEndDateWithLastestOperationalStartDate_WhenStagingContainsMultipleUpdatedDataForSameStandard(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, int operationalEndDateOffsetDays, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithOfqualStandard(ofqualStandardId, recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber, currentDateTime.AddDays(-1), null)
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber)
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate.AddDays(5), operationalEndDate?.AddDays(operationalEndDateOffsetDays), ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                // if the operational end date is not changing then no update will be made
                var expectedUpdatedAt = operationalEndDate == operationalEndDate?.AddDays(operationalEndDateOffsetDays)
                    ? (DateTime?)null
                    : currentDateTime;

                await results.VerifyOfqualStandardExists(OfqualStandardHandler.Create(ofqualStandardId, recognitionNumber, operationalStartDate, 
                    operationalEndDate?.AddDays(operationalEndDateOffsetDays), ifateReferenceNumber, currentDateTime.AddDays(-1), expectedUpdatedAt));
            }
        }

        /*[TestCase("RN0001", "2020-01-01", "2021-02-01", "ST0001")]
        public async Task LoadOfqalStandards_AddStandardToOrganisationStandards_WhenStagingContainsAddedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqalStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", null, "Ofqual")
                .WithOrganisation("OrganisationOne", "EPA0001", 12345678, "RN0001")
                .WithStagingOfqualOrganisation("RN0001", "OrganisationOne", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
                    "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
                    "OfqualStatus", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1))
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                await results.VerifyOrganisationStandardExists(OrganisationStandardHandler.Create(null, "EPA0001", 101, new DateTime(2020, 1, 1), new DateTime(2021, 2, 1), 
                    currentDateTime, "Added from OFQUAL qualifications list", "Live", "ST0001"));
            }
        }*/

        // TO DO: do not update a standard by a change in the Ofqual standard information

        private class LoadOfqalStandardsTestsFixture : FixtureBase<LoadOfqalStandardsTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private StandardRepository _repository;
            public int _updated;

            public LoadOfqalStandardsTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
                _repository = new StandardRepository(new UnitOfWork(_sqlConnection));
            }

            public async Task<LoadOfqalStandardsTestsFixture> LoadOfqualStandards(DateTime dateTimeUtc)
            {
                _updated = await _repository.LoadOfqualStandards(dateTimeUtc);
                return this;
            }

            public async Task<LoadOfqalStandardsTestsFixture> VerifyOfqualOrganisationExists(OfqualOrganisationModel ofqualOrganisation)
            {
                var result = await OfqualOrganisationHandler.QueryFirstOrDefaultAsync(ofqualOrganisation);
                result.Should().NotBeNull();

                return this;
            }

            public async Task<LoadOfqalStandardsTestsFixture> VerifyOfqualStandardExists(OfqualStandardModel ofqualStandard)
            {
                var result = await OfqualStandardHandler.QueryFirstOrDefaultAsync(ofqualStandard);
                result.Should().NotBeNull();

                return this;
            }
        }
    }
}

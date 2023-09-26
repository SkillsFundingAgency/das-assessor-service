using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    public class LoadOfqualStandardsTests : TestBase
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
        public async Task LoadOfqualStandards_UpdateOfqualOrganiation_WhenStagingContainsUpdatedData(
            string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime ofqualRecognisedTo)
        {
            var ofqualOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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
        public async Task LoadOfqualStandards_AddOfqualOrganiation_WhenStagingContainsAddedData(
            string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime ofqualRecognisedTo)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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
        public async Task LoadOfqualStandards_DoesNotRemoveOfqualOrganiation_WhenStagingContainsRemovedData(
            string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime ofqualRecognisedTo)
        {
            var ofqualOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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
        public async Task LoadOfqualStandards_UpdateOfqualStandards_WhenStagingContainsUpdatedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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
        public async Task LoadOfqualStandards_AddOfqualStandards_WhenStagingContainsAddedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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
        public async Task LoadOfqualStandards_DoesNotRemoveOfqualStandards_WhenStagingContainsRemovedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardIdOne = Guid.NewGuid();
            var ofqualStandardIdTwo = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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
        public async Task LoadOfqualStandards_UpdateOfqualStandardsTakesEarlierOperationalStartDate_WhenStagingContainsMultipleUpdatedDataForSameStandard(
            string recognitionNumber, DateTime operationalStartDate, int operationalStartDateOffsetDays, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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
        public async Task LoadOfqualStandards_UpdateOfqualStandardsTakesOperationalEndDateWithLastestOperationalStartDate_WhenStagingContainsMultipleUpdatedDataForSameStandard(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, int operationalEndDateOffsetDays, string ifateReferenceNumber)
        {
            var ofqualStandardId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
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

        [TestCase("RN0001", "2020-01-01", "2021-02-01", "ST0001")]
        [TestCase("RN0002", "2020-12-01", "2021-06-01", "ST0002")]
        [TestCase("RN0003", "2021-06-01", "2022-08-01", "ST0003")]
        public async Task LoadOfqualStandards_AddStandardToOrganisationStandards_WhenStagingContainsAddedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, 101, "1.0", null, true, "Ofqual")
                .WithOrganisation("OrganisationOne", "EPA0001", 12345678, recognitionNumber)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithStagingOfqualOrganisation(recognitionNumber, "OrganisationOne", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
                    "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
                    "OfqualStatus", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1))
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                var expected = await OrganisationStandardHandler.Create(null, "EPA0001", 101, operationalStartDate, operationalEndDate,
                    currentDateTime.Date, "Added from OFQUAL qualifications list", "displayname@organisationone.com", "Live", ifateReferenceNumber);

                results.VerifyUpdated(1);
                await results.VerifyOrganisationStandardRowCount(1);
                await results.VerifyOrganisationStandardExists(expected);
            }
        }

        [TestCase("RN0001", "2020-01-01", "2021-02-01", "ST0001")]
        [TestCase("RN0002", "2020-12-01", "2021-06-01", "ST0002")]
        [TestCase("RN0003", "2021-06-01", "2022-08-01", "ST0003")]
        public async Task LoadOfqualStandards_DoNotAddStandardToOrganisationStandards_WhenStagingContainsAddedDataForNonOfqualStandard(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, 101, "1.0", null, true, "Office for Students")
                .WithOrganisation("OrganisationOne", "EPA0001", 12345678, recognitionNumber)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithStagingOfqualOrganisation(recognitionNumber, "OrganisationOne", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
                    "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
                    "OfqualStatus", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1))
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                results.VerifyUpdated(0);
                await results.VerifyOrganisationStandardRowCount(0);
            }
        }

        [TestCase("RN0001", "2020-01-01", "2021-02-01", "ST0001")]
        [TestCase("RN0002", "2020-12-01", "2021-06-01", "ST0002")]
        [TestCase("RN0003", "2021-06-01", "2022-08-01", "ST0003")]
        public async Task LoadOfqualStandards_DoNotAddStandardToOrganisationStandards_WhenStagingContainsExistingStandardData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, 101, "1.0", null, true, "Ofqual")
                .WithOrganisation("OrganisationOne", "EPA0001", 12345678, recognitionNumber)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithOrganisationStandard(1, "EPA0001", 101, ifateReferenceNumber, operationalStartDate.AddDays(-15), operationalEndDate?.AddDays(5), currentDateTime.Date.AddDays(-10))
                .WithStagingOfqualOrganisation(recognitionNumber, "OrganisationOne", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
                    "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
                    "OfqualStatus", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1))
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                var expectedResult = OrganisationStandardHandler.Create(null, "EPA0001", 101, operationalStartDate.AddDays(-15), operationalEndDate?.AddDays(5),
                    currentDateTime.Date.AddDays(-10), string.Empty, null as Guid?, "Live", ifateReferenceNumber);

                results.VerifyUpdated(0);
                await results.VerifyOrganisationStandardRowCount(1);
                await results.VerifyOrganisationStandardExists(expectedResult);
            }
        }

        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0001", new[] { "1.0", "1.1", "1.2" })]
        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0002", new[] { "1.0", "1.1", "1.2" })]
        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0003", new[] { "1.1", "1.2" })]
        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0004", new[] { "1.2" })]
        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0005", new[] { "1.2" })]
        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0006", new[] { "1.2" })]
        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0007", new[] { "1.1", "1.2" })]
        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0008", new[] { "1.2" })]
        public async Task LoadOfqualStandards_AddOrganisationStandardVersionsForLatestEpaPlan_WhenStagingContainsAddedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber, string[] versionsWithLatestEpaPlan)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
                .WithStandard("One", "ST0001", 101, "1.0", null, false, "Ofqual")
                .WithStandard("One", "ST0001", 101, "1.1", null, false, "Ofqual")
                .WithStandard("One", "ST0001", 101, "1.2", null, false, "Ofqual")
                .WithStandard("Two", "ST0002", 102, "1.0", null, true, "Ofqual")
                .WithStandard("Two", "ST0002", 102, "1.1", null, false, "Ofqual")
                .WithStandard("Two", "ST0002", 102, "1.2", null, false, "Ofqual")
                .WithStandard("Three", "ST0003", 103, "1.0", null, false, "Ofqual")
                .WithStandard("Three", "ST0003", 103, "1.1", null, true, "Ofqual")
                .WithStandard("Three", "ST0003", 103, "1.2", null, false, "Ofqual")
                .WithStandard("Four", "ST0004", 104, "1.0", null, false, "Ofqual")
                .WithStandard("Four", "ST0004", 104, "1.1", null, false, "Ofqual")
                .WithStandard("Four", "ST0004", 104, "1.2", null, true, "Ofqual")
                .WithStandard("Five", "ST0005", 105, "1.0", null, false, "Ofqual")
                .WithStandard("Five", "ST0005", 105, "1.1", null, true, "Ofqual")
                .WithStandard("Five", "ST0005", 105, "1.2", null, true, "Ofqual")
                .WithStandard("Six", "ST0006", 106, "1.0", null, true, "Ofqual")
                .WithStandard("Six", "ST0006", 106, "1.1", null, false, "Ofqual")
                .WithStandard("Six", "ST0006", 106, "1.2", null, true, "Ofqual")
                .WithStandard("Seven", "ST0007", 107, "1.0", null, true, "Ofqual")
                .WithStandard("Seven", "ST0007", 107, "1.1", null, true, "Ofqual")
                .WithStandard("Seven", "ST0007", 107, "1.2", null, false, "Ofqual")
                .WithStandard("Eight", "ST0008", 108, "1.0", null, true, "Ofqual")
                .WithStandard("Eight", "ST0008", 108, "1.1", null, true, "Ofqual")
                .WithStandard("Eight", "ST0008", 108, "1.2", null, true, "Ofqual")
                .WithOrganisation("OrganisationOne", "EPA0001", 12345678, recognitionNumber)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithStagingOfqualOrganisation(recognitionNumber, "OrganisationOne", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
                    "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
                    "OfqualStatus", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1))
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                results.VerifyUpdated(1);
                await results.VerifyOrganisationStandardRowCount(1);
                await results.VerifyOrganisationStandardVersionRowCount(versionsWithLatestEpaPlan.Count());

                foreach (var version in versionsWithLatestEpaPlan)
                {
                    var expectedResult = OrganisationStandardVersionHandler.Create($"{ifateReferenceNumber}_{version}", version, null, operationalStartDate,
                        null, currentDateTime.Date, "Added from OFQUAL qualifications list", "Live");

                    await results.VerifyOrganisationStandardVersionExists(expectedResult);
                }
            }
        }

        [TestCase("RN0001", "2021-06-01", "2022-08-01", "ST0001")]
        public async Task LoadOfqualStandards_AddOrganisationStandardDeliveryArea_WhenStagingContainsAddedData(
            string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber)
        {
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfqualStandardsTestsFixture()
                .WithStandard("One", "ST0001", 101, "1.0", null, false, "Ofqual")
                .WithOrganisation("OrganisationOne", "EPA0001", 12345678, recognitionNumber)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithStagingOfqualOrganisation(recognitionNumber, "OrganisationOne", "LegalName", "Acronym", "Email", "Website", "HeadOfficeAddressLine1", "HeadOfficeAddressLine2",
                    "HeadOfficeAddressTown", "HeadOfficeAddressCounty", "Postcode", "HeadOfficeAddressCountry", "HeadOfficeAddressTelephone",
                    "OfqualStatus", new DateTime(2020, 1, 1), new DateTime(2020, 2, 1))
                .WithStagingOfqualStandard(recognitionNumber, operationalStartDate, operationalEndDate, ifateReferenceNumber))
            {
                var results = await fixture.LoadOfqualStandards(currentDateTime);

                results.VerifyUpdated(1);
                await results.VerifyOrganisationStandardRowCount(1);
                await results.VerifyOrganisationStandardDeliveryAreaRowCount(9);
            }
        }

        private class LoadOfqualStandardsTestsFixture : FixtureBase<LoadOfqualStandardsTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private StandardRepository _repository;
            public int _updated;

            public LoadOfqualStandardsTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
                _repository = new StandardRepository(new UnitOfWork(_sqlConnection));
            }

            public async Task<LoadOfqualStandardsTestsFixture> LoadOfqualStandards(DateTime dateTimeUtc)
            {
                _updated = await _repository.LoadOfqualStandards(dateTimeUtc);
                return this;
            }

            public LoadOfqualStandardsTestsFixture VerifyUpdated(int updated)
            {
                _updated.Should().Be(updated);
                return this;
            }
        }
    }
}

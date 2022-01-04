using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.IntegrationTests.Services
{
    public class OrganisationMergingServiceTests : TestBase
    {
        [Test]
        public async Task CreateMergeOrganisations_Should_BePopulatedCorrectlyAndAutoApproved()
        {
            // Arrange.

            var primaryEpaId = "EPA0200";
            var primaryContactId = Guid.NewGuid();
            CreateOrganisationWithContact(primaryEpaId, "Primary Organisation Name", primaryContactId, "Primary Contact Name", "contact@primary.com");

            var secondaryEpaId = "EPA0201";
            var secondaryContactId = Guid.NewGuid();
            CreateOrganisationWithContact(secondaryEpaId, "Secondary Organisation Name", secondaryContactId, "Secondary Contact Name", "contact@secondary.com");

            // Act.

            var userId = Guid.NewGuid();
            var dbContext = DatabaseHelper.TestContext;
            var sut = new OrganisationMergingService(dbContext);
            var testExcutionTimestamp = DateTime.UtcNow;
            var mo = sut.CallPrivateMethod<MergeOrganisation>("CreateMergeOrganisations", new object[] { primaryEpaId, secondaryEpaId, userId });
            await dbContext.SaveChangesAsync();

            // Assert.

            mo.Id.Should().BeGreaterThan(0);
            mo.PrimaryEndPointAssessorOrganisationId.Should().Be(primaryEpaId);
            mo.SecondaryEndPointAssessorOrganisationId.Should().Be(secondaryEpaId);
            mo.CreatedAt.Should().BeCloseTo(testExcutionTimestamp, 500);
            mo.UpdatedAt.Should().BeNull();
            mo.Status.Should().Be(MergeOrganisationStatus.Approved);
            mo.CreatedBy.Should().Be(userId);
            mo.UpdatedBy.Should().BeNull();
            mo.ApprovedAt.Should().BeCloseTo(testExcutionTimestamp, 500);
            mo.ApprovedBy.Should().Be(userId);
        }


        [Test]
        public async Task MergeOrganisations_Should_CreateBeforeSnapshot()
        {
            // @ToDo need tests for scenarios where standards + versions are distinct for both organsations, and for where there is overlap

            //
            //  Arrange.
            //

            var primaryEpaId = "EPA0200";
            var primaryContactId = Guid.NewGuid();
            CreateOrganisationWithContact(primaryEpaId, "Primary Organisation Name", primaryContactId, "Primary Contact Name", "primarycontact@primary.com");
            var id = CreateOrganisationStandard(primaryEpaId, 271, "ST0190", primaryContactId);
            CreateOrganisationStandardVersion(id, "ST0190", "1.0");
            CreateOrganisationStandardDeliveryArea(id, 1);
            CreateOrganisationStandardDeliveryArea(id, 2);
            CreateOrganisationStandardDeliveryArea(id, 3);
            CreateOrganisationStandardDeliveryArea(id, 4);
            CreateOrganisationStandardDeliveryArea(id, 5);
            CreateOrganisationStandardDeliveryArea(id, 6);
            CreateOrganisationStandardDeliveryArea(id, 7);
            CreateOrganisationStandardDeliveryArea(id, 8);
            CreateOrganisationStandardDeliveryArea(id, 9);

            var secondaryEpaId = "EPA0201";
            var secondaryContactId = Guid.NewGuid();
            CreateOrganisationWithContact(secondaryEpaId, "Secondary Organisation Name", secondaryContactId, "Secondary Contact Name", "contact@secondary.com");
            id = CreateOrganisationStandard(secondaryEpaId, 139, "ST0232", secondaryContactId);
            CreateOrganisationStandardVersion(id, "ST0232", "1.0");
            CreateOrganisationStandardDeliveryArea(id, 1);
            CreateOrganisationStandardDeliveryArea(id, 2);
            CreateOrganisationStandardDeliveryArea(id, 3);
            CreateOrganisationStandardDeliveryArea(id, 4);
            CreateOrganisationStandardDeliveryArea(id, 5);
            CreateOrganisationStandardDeliveryArea(id, 6);
            CreateOrganisationStandardDeliveryArea(id, 7);
            CreateOrganisationStandardDeliveryArea(id, 8);
            CreateOrganisationStandardDeliveryArea(id, 9);

            id = CreateOrganisationStandard(secondaryEpaId, 271, "ST0190", secondaryContactId);
            CreateOrganisationStandardVersion(id, "ST0190", "1.1");
            CreateOrganisationStandardDeliveryArea(id, 1);
            CreateOrganisationStandardDeliveryArea(id, 2);
            CreateOrganisationStandardDeliveryArea(id, 3);
            CreateOrganisationStandardDeliveryArea(id, 4);
            CreateOrganisationStandardDeliveryArea(id, 5);
            CreateOrganisationStandardDeliveryArea(id, 6);
            CreateOrganisationStandardDeliveryArea(id, 7);
            CreateOrganisationStandardDeliveryArea(id, 8);
            CreateOrganisationStandardDeliveryArea(id, 9);

            //
            //  Act.
            //

            var dbContext = DatabaseHelper.TestContext;
            var primaryOrganisation = dbContext.Organisations.First(e => e.EndPointAssessorOrganisationId == primaryEpaId);
            var secondaryOrganisation = dbContext.Organisations.First(e => e.EndPointAssessorOrganisationId == secondaryEpaId);
            var userId = Guid.NewGuid();
            var sut = new OrganisationMergingService(dbContext);
            var mo = await sut.MergeOrganisations(primaryOrganisation, secondaryOrganisation, userId);

            //
            //  Assert.
            //

            mo.Should().NotBeNull();
            mo.MergeOrganisationStandards.Should().HaveCount(3);
            mo.MergeOrganisationStandardVersions.Should().HaveCount(3);
            mo.MergeOrganisationStandardDeliveryAreas.Should().HaveCount(27);
        }
    }
}

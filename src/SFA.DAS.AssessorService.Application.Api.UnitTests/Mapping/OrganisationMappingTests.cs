using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Mapping
{
    [TestFixture]
    public class OrganisationMappingTests : TestBase
    {
        [Test, RecursiveMoqAutoData]
        public void ShouldMapOrganisationToOrganisationStandardResponse(Organisation organisation)
        {
            // Act
            var response = Mapper.Map<OrganisationStandardResponse>(organisation);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(organisation.Id);
            response.PrimaryContact.Should().Be(organisation.PrimaryContact);
            response.Status.Should().Be(organisation.Status);
            response.EndPointAssessorName.Should().Be(organisation.EndPointAssessorName);
            response.EndPointAssessorOrganisationId.Should().Be(organisation.EndPointAssessorOrganisationId);
            response.EndPointAssessorUkprn.Should().Be(organisation.EndPointAssessorUkprn);
            response.OrganisationType.Should().Be(organisation.OrganisationType?.Type);
            response.City.Should().Be(organisation.OrganisationData?.Address4);
            response.Postcode.Should().Be(organisation.OrganisationData?.Postcode);

            var expectedStandard = organisation.OrganisationStandards.FirstOrDefault();

            response.OrganisationStandard.Should().NotBeNull();
            response.OrganisationStandard.StandardId.Should().Be(expectedStandard.StandardCode);
            response.OrganisationStandard.EffectiveFrom.Should().Be(expectedStandard.EffectiveFrom);
            response.OrganisationStandard.EffectiveTo.Should().Be(expectedStandard.EffectiveTo);
            response.OrganisationStandard.DateStandardApprovedOnRegister.Should().Be(expectedStandard.DateStandardApprovedOnRegister);

            var expectedMappedDeliveryAreas = expectedStandard.OrganisationStandardDeliveryAreas.Select(x =>
                new AssessorService.Api.Types.Models.AO.OrganisationStandardDeliveryArea
                {
                    Id = x.Id,
                    DeliveryArea = x.DeliveryArea.Area,
                    Status = x.Status,
                    DeliveryAreaId = x.DeliveryArea.Id
                }).ToList();

            response.DeliveryAreasDetails.Should().BeEquivalentTo(expectedMappedDeliveryAreas);

            var properties = typeof(OrganisationStandardResponse).GetProperties();
            var mappedFields = new HashSet<string>
            {
                "Id", "PrimaryContact", "Status", "EndPointAssessorName",
                "EndPointAssessorOrganisationId", "EndPointAssessorUkprn",
                "OrganisationType", "City", "Postcode", "DeliveryAreasDetails",
                "OrganisationStandard"
            };

            foreach (var property in properties)
            {
                if (!mappedFields.Contains(property.Name))
                {
                    var value = property.GetValue(response);
                    if (property.PropertyType == typeof(bool))
                    {
                        ((bool)value).Should().BeFalse($"Unmapped property {property.Name} should default to false");
                    }
                    else
                    {
                        value.Should().BeNull($"Unmapped property {property.Name} should not be mapped and should be null");
                    }
                }
            }
        }


        [Test, RecursiveMoqAutoData]
        public void ShouldMapListOfOrganisationsToOrganisationStandardResponseList(List<Organisation> organisations)
        {
            // Act
            var responses = Mapper.Map<List<OrganisationStandardResponse>>(organisations);

            // Assert
            responses.Should().NotBeNull();
            responses.Count.Should().Be(organisations.Count);

            for (int i = 0; i < responses.Count; i++)
            {
                ShouldMapOrganisationToOrganisationStandardResponse(organisations[i]);
            }
        }

        [Test, RecursiveMoqAutoData]
        public void ShouldMapOrganisationToOrganisationResponse(Organisation organisation)
        {
            // Act
            var response = Mapper.Map<OrganisationResponse>(organisation);

            //Assert
            response.Id.Should().Be(organisation.Id);
            response.PrimaryContact.Should().Be(organisation.PrimaryContact);
            response.Status.Should().Be(organisation.Status);
            response.EndPointAssessorName.Should().Be(organisation.EndPointAssessorName);
            response.EndPointAssessorOrganisationId.Should().Be(organisation.EndPointAssessorOrganisationId);
            response.EndPointAssessorUkprn.Should().Be(organisation.EndPointAssessorUkprn);
            response.RoATPApproved.Should().Be(organisation.OrganisationData.RoATPApproved);
            response.RoEPAOApproved.Should().Be(organisation.OrganisationData.RoEPAOApproved);
            response.OrganisationType.Should().Be(organisation.OrganisationType?.Type);
            response.CompanySummary.Should().Be(organisation.OrganisationData?.CompanySummary);
            response.CharitySummary.Should().Be(organisation.OrganisationData?.CharitySummary);

            var properties = typeof(OrganisationResponse).GetProperties();
            foreach (var property in properties)
            {
                var mappedFields = new HashSet<string>
                {
                    "Id", "PrimaryContact", "Status", "EndPointAssessorName",
                    "EndPointAssessorOrganisationId", "EndPointAssessorUkprn",
                    "RoATPApproved", "RoEPAOApproved", "OrganisationType",
                    "CompanySummary", "CharitySummary"
                };

                if (!mappedFields.Contains(property.Name))
                {
                    var value = property.GetValue(response);
                    value.Should().BeNull($"Property {property.Name} should not be mapped and should be null");
                }
            }
        }

        [Test, RecursiveMoqAutoData]
        public void ShouldMapListOfOrganisationsToOrganisationResponseList(List<Organisation> organisations)
        {
            // Act
            var responses = Mapper.Map<List<OrganisationResponse>>(organisations);

            // Assert
            responses.Should().NotBeNull();
            responses.Count.Should().Be(organisations.Count);

            for (int i = 0; i < responses.Count; i++)
            {
                ShouldMapOrganisationToOrganisationResponse(organisations[i]);
            }
        }
    }
}

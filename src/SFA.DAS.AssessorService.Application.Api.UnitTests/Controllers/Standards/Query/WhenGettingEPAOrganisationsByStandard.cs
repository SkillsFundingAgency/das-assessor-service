using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards.Query
{
    public class WhenGettingEPAOrganisationsByStandard : TestBase
    {
        protected Mock<ILogger<StandardQueryController>> ControllerLoggerMock;
        protected StandardQueryController Controller;
        protected Mock<IMediator> Mediator = new Mock<IMediator>();

        [SetUp]
        protected  void Setup()
        {
            ControllerLoggerMock = new Mock<ILogger<StandardQueryController>>();
            Controller = new StandardQueryController(Mediator.Object, ControllerLoggerMock.Object, Mapper);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_The_Standard_Id_Is_Not_Supplied_A_Bad_Request_Is_Returned()
        {
            //Act
            var actual = await Controller.GetEpaosByStandard(0);

            //Assert
            actual.Should().NotBeNull();
            var actualResult = actual as BadRequestResult;
            actualResult.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_No_Data_Returned_A_Not_Found_Result_Is_Returned(
            int standardCode)
        {
            //Arrange
            Mediator
                .Setup(x => x.Send(
                    It.Is<GetEpaOrganisationsByStandardQuery>(c=>c.Standard.Equals(standardCode)), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEpaOrganisationsByStandardResponse
                {
                    EpaOrganisations = new List<Organisation>()
                });

            //Act
            var actual = await Controller.GetEpaosByStandard(standardCode);

            //Assert
            actual.Should().NotBeNull();
            var actualResult = actual as NotFoundResult;
            actualResult.Should().NotBeNull();
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_If_There_Is_Data_It_Is_Returned_In_Response(
            int standardCode,
            List<Organisation> epaOrganisations)
        {
            //Arrange
            Mediator
                .Setup(x => x.Send(
                    It.Is<GetEpaOrganisationsByStandardQuery>(c => c.Standard.Equals(standardCode)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEpaOrganisationsByStandardResponse
                {
                    EpaOrganisations = epaOrganisations
                });

            //Act
            var actual = await Controller.GetEpaosByStandard(standardCode);

            //Assert
            actual.Should().NotBeNull();
            var actualResult = actual as OkObjectResult;
            actualResult.Should().NotBeNull();
            var actualModel = actualResult.Value as List<OrganisationStandardResponse>;

            for (int i = 0; i < actualModel.Count; i++)
            {
                var response = actualModel[i];
                var expected = epaOrganisations.Where(o => o.Id == response.Id).FirstOrDefault();

                response.Id.Should().Be(expected.Id);
                response.PrimaryContact.Should().Be(expected.PrimaryContact);
                response.Status.Should().Be(expected.Status);
                response.EndPointAssessorName.Should().Be(expected.EndPointAssessorName);
                response.EndPointAssessorOrganisationId.Should().Be(expected.EndPointAssessorOrganisationId);
                response.EndPointAssessorUkprn.Should().Be(expected.EndPointAssessorUkprn);
                response.OrganisationType.Should().Be(expected.OrganisationType?.Type);
                response.City.Should().Be(expected.OrganisationData?.Address4);
                response.Postcode.Should().Be(expected.OrganisationData?.Postcode);

                var expectedStandard = expected.OrganisationStandards.FirstOrDefault();

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

                var properties = typeof(OrganisationResponse).GetProperties();
                foreach (var property in properties)
                {
                    var mappedFields = new HashSet<string>
                    {
                        "Id", "PrimaryContact", "Status", "EndPointAssessorName",
                        "EndPointAssessorOrganisationId", "EndPointAssessorUkprn",
                        "OrganisationType","City", "Postcode", "DeliveryAreaDetails",
                        "OrganisationStandard"
                    };

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
        }

        
    }
}
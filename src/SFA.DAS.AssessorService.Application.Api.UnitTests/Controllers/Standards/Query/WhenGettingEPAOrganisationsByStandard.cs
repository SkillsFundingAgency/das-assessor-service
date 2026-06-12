using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
    public class WhenGettingEPAOrganisationsByStandard
    {
        protected Mock<ILogger<StandardQueryController>> LoggerMock = new Mock<ILogger<StandardQueryController>>();
        protected Mock<IMediator> MediatorMock = new Mock<IMediator>();
        protected Mock<IMapper> MapperMock = new Mock<IMapper>();
        protected StandardQueryController Controller;

        [SetUp]
        protected  void Setup()
        {
            Controller = new StandardQueryController(MediatorMock.Object, LoggerMock.Object, MapperMock.Object);
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
            MediatorMock
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
            MediatorMock
                .Setup(x => x.Send(
                    It.Is<GetEpaOrganisationsByStandardQuery>(c => c.Standard.Equals(standardCode)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEpaOrganisationsByStandardResponse
                {
                    EpaOrganisations = epaOrganisations
                });

            var mappedOrganisations = new List<OrganisationStandardResponse>(); 

            MapperMock.Setup(m => m.Map<List<OrganisationStandardResponse>>(epaOrganisations)) 
                .Returns(mappedOrganisations); 

            //Act
            var actual = await Controller.GetEpaosByStandard(standardCode);

            //Assert
            actual.Should().NotBeNull();
            var actualResult = actual as OkObjectResult;
            actualResult.Should().NotBeNull();
            var actualModel = actualResult.Value as List<OrganisationStandardResponse>;

            MapperMock.Verify(m => m.Map<List<OrganisationStandardResponse>>(epaOrganisations), Times.Once); 

 
        }

        
    }
}
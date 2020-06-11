﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards.Query
{
    public class WhenGettingEPAOrganisationsByStandard
    {
        [Test, MoqAutoData]
        public async Task Then_If_The_Standard_Id_Is_Not_Supplied_A_Bad_Request_Is_Returned(
            StandardQueryController controller)
        {
            //Act
            var actual = await controller.GetEpaosByStandard(0);

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as BadRequestResult;
            Assert.IsNotNull(actualResult);
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_No_Data_Returned_A_Not_Found_Result_Is_Returned(
            int standardCode,
            [Frozen] Mock<IMediator> mediator,
            StandardQueryController controller)
        {
            //Arrange
            mediator
                .Setup(x => x.Send(
                    It.Is<GetEpaOrganisationsByStandardQuery>(c=>c.Standard.Equals(standardCode)), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEpaOrganisationsByStandardResponse
                {
                    EpaOrganisations = new List<OrganisationResponse>()
                });

            //Act
            var actual = await controller.GetEpaosByStandard(standardCode);

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as NotFoundResult;
            Assert.IsNotNull(actualResult);
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_Data_It_Is_Returned_In_Response(
            int standardCode,
            List<OrganisationResponse> epaOrganisations,
            [Frozen] Mock<IMediator> mediator,
            StandardQueryController controller)
        {
            //Arrange
            mediator
                .Setup(x => x.Send(
                    It.Is<GetEpaOrganisationsByStandardQuery>(c => c.Standard.Equals(standardCode)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEpaOrganisationsByStandardResponse
                {
                    EpaOrganisations = epaOrganisations
                });

            //Act
            var actual = await controller.GetEpaosByStandard(standardCode);

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as OkObjectResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Value as List<OrganisationResponse>;
            actualModel.Should().BeEquivalentTo(epaOrganisations);
        }
    }
}
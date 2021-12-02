using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards.Pipelines
{
    public class WhenGettingPipelineFilters
    {
        private PaginatedList<EpaoPipelineStandardsResponse> validPaginatedResponse = new PaginatedList<EpaoPipelineStandardsResponse>(
            new List<EpaoPipelineStandardsResponse>()
            {
                new EpaoPipelineStandardsResponse() { StandardCode = "271", StandardName = "Advanced Baker", StandardVersion = "1.0", UKPRN = "10026402", TrainingProvider = "ACTIONS LIMTED", Pipeline = 1, EstimatedDate = "2021-08-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "271", StandardName = "Advanced Baker", StandardVersion = "1.1", UKPRN = "10026402", TrainingProvider = "ACTIONS LIMTED", Pipeline = 1, EstimatedDate = "2021-09-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "271", StandardName = "Advanced Baker", StandardVersion = "1.2", UKPRN = "10026402", TrainingProvider = "ACTIONS LIMTED", Pipeline = 1, EstimatedDate = "2021-10-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "271", StandardName = "Advanced Baker", StandardVersion = "1.0", UKPRN = "10061684", TrainingProvider = "ALL SPRING MEDIA LIMITED", Pipeline = 1, EstimatedDate = "2021-08-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "303", StandardName = "Asbestos analyst and surveyor", StandardVersion = "1.0", UKPRN = "10026402", TrainingProvider = "ACTIONS LIMTED", Pipeline = 1, EstimatedDate = "2021-08-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "303", StandardName = "Asbestos analyst and surveyor", StandardVersion = "1.0", UKPRN = "10037442", TrainingProvider = "ADVANCED CARE YORKSHIRE LIMTED", Pipeline = 1, EstimatedDate = "2021-08-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.0", UKPRN = "10000460", TrainingProvider = "AUTOMOTIVE TRANSPORT TRAINING LIMITED", Pipeline = 1, EstimatedDate = "2021-11-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.1", UKPRN = "10000460", TrainingProvider = "AUTOMOTIVE TRANSPORT TRAINING LIMITED", Pipeline = 1, EstimatedDate = "2021-11-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.2", UKPRN = "10000460", TrainingProvider = "AUTOMOTIVE TRANSPORT TRAINING LIMITED", Pipeline = 1, EstimatedDate = "2021-11-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.3", UKPRN = "10000460", TrainingProvider = "AUTOMOTIVE TRANSPORT TRAINING LIMITED", Pipeline = 1, EstimatedDate = "2021-11-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.0", UKPRN = "10057058", TrainingProvider = "L C PARTNERSHIP LTD", Pipeline = 1, EstimatedDate = "2021-08-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.1", UKPRN = "10057058", TrainingProvider = "L C PARTNERSHIP LTD", Pipeline = 1, EstimatedDate = "2021-09-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.2", UKPRN = "10057058", TrainingProvider = "L C PARTNERSHIP LTD", Pipeline = 1, EstimatedDate = "2021-10-12 00:00:00.000" },
                new EpaoPipelineStandardsResponse() { StandardCode = "54", StandardName = "Butcher", StandardVersion = "1.3", UKPRN = "10057058", TrainingProvider = "L C PARTNERSHIP LTD", Pipeline = 1, EstimatedDate = "2021-11-12 00:00:00.000" },
            }
            , 14, 1, 10);

        [Test, MoqAutoData]
        public async Task Then_If_OrgId_Not_Supplied_Return_BadRequest(
            [Frozen] Mock<IMediator> mediator,
            StandardQueryController controller)
        {
            // Arrange

            var epaoId = string.Empty;
            mediator
                .Setup(x => x.Send(
                    It.Is<EpaoPipelineStandardsFiltersRequest>(c => c.EpaoId.Equals(epaoId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EpaoPipelineStandardsFiltersResponse() { });

            // Act

            var actual = await controller.GetEpaoPipelineStandardsFilters(epaoId);

            // Assert

            actual.Should().NotBeNull();
            var actualResult = actual as BadRequestResult;
            actualResult.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_If_No_Pipeline_Data_Return_Ok_No_Data(
            string epaoId,
            [Frozen] Mock<IMediator> mediator,
            StandardQueryController controller)
        {
            // Arrange

            mediator
                .Setup(x => x.Send(
                    It.Is<EpaoPipelineStandardsFiltersRequest>(c => c.EpaoId.Equals(epaoId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EpaoPipelineStandardsFiltersResponse() 
                {
                    StandardFilterItems = new List<EpaoPipelineStandardFilter>(),
                    ProviderFilterItems = new List<EpaoPipelineStandardFilter>(),
                    EPADateFilterItems = new List<EpaoPipelineStandardFilter>(),
                });

            // Act

            var actual = await controller.GetEpaoPipelineStandardsFilters(epaoId);

            // Assert

            actual.Should().NotBeNull();
            var actualResult = actual as OkObjectResult;
            actualResult.Should().NotBeNull();
            var actualModel = actualResult.Value as EpaoPipelineStandardsFiltersResponse;
            actualModel.StandardFilterItems.Should().NotBeNull();
            actualModel.StandardFilterItems.Should().BeEmpty();
            actualModel.ProviderFilterItems.Should().NotBeNull();
            actualModel.ProviderFilterItems.Should().BeEmpty();
            actualModel.EPADateFilterItems.Should().NotBeNull();
            actualModel.EPADateFilterItems.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task Then_If_Pipeline_Data_Exists_Return_Ok_With_Filters(
            string epaoId,
            [Frozen] Mock<IMediator> mediator,
            StandardQueryController controller)
        {
            // Arrange

            mediator
                .Setup(x => x.Send(
                    It.Is<EpaoPipelineStandardsFiltersRequest>(c => c.EpaoId.Equals(epaoId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EpaoPipelineStandardsFiltersResponse()
                {
                    StandardFilterItems = new List<EpaoPipelineStandardFilter>()
                    {
                        new EpaoPipelineStandardFilter() { Id = "271", Value = "Advanced Baker" },
                        new EpaoPipelineStandardFilter() { Id = "303", Value = "Asbestos analyst and surveyor" },
                        new EpaoPipelineStandardFilter() { Id = "54", Value = "Butcher" },
                    },
                    ProviderFilterItems = new List<EpaoPipelineStandardFilter>() {
                        new EpaoPipelineStandardFilter() { Id = "10026402", Value = "ACTIONS LIMTED" },
                        new EpaoPipelineStandardFilter() { Id = "10061684", Value = "ALL SPRING MEDIA LIMITED" },
                        new EpaoPipelineStandardFilter() { Id = "10037442", Value = "ADVANCED CARE YORKSHIRE LIMTED" },
                        new EpaoPipelineStandardFilter() { Id = "10000460", Value = "AUTOMOTIVE TRANSPORT TRAINING LIMITED" },
                        new EpaoPipelineStandardFilter() { Id = "10057058", Value = "L C PARTNERSHIP LTD" },
                    },
                    EPADateFilterItems = new List<EpaoPipelineStandardFilter>()
                    {
                        new EpaoPipelineStandardFilter() { Id = "202108", Value = "August 2021" },
                        new EpaoPipelineStandardFilter() { Id = "202109", Value = "September 2021" },
                        new EpaoPipelineStandardFilter() { Id = "202110", Value = "October 2021" },
                        new EpaoPipelineStandardFilter() { Id = "202111", Value = "November 2021" },
                        new EpaoPipelineStandardFilter() { Id = "202112", Value = "December 2021" },
                    }
                });

            // Act

            var actual = await controller.GetEpaoPipelineStandardsFilters(epaoId);

            // Assert

            actual.Should().NotBeNull();
            var actualResult = actual as OkObjectResult;
            actualResult.Should().NotBeNull();
            var actualModel = actualResult.Value as EpaoPipelineStandardsFiltersResponse;
            actualModel.StandardFilterItems.Should().NotBeNull();
            actualModel.StandardFilterItems.Should().HaveCount(3);
            actualModel.ProviderFilterItems.Should().NotBeNull();
            actualModel.ProviderFilterItems.Should().HaveCount(5);
            actualModel.EPADateFilterItems.Should().NotBeNull();
            actualModel.EPADateFilterItems.Should().HaveCount(5);
        }
    }
}

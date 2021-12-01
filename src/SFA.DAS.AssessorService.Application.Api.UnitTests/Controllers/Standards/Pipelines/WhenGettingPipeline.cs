using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards.Pipelines
{
    public class WhenGettingPipeline
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
            string standardFilterId,
            string providerFilterId,
            string EPADateFilterId,
            string orderBy,
            string orderDirection,
            int pageSize,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] StandardQueryController controller)
        {
            // Arrange

            var epaoId = string.Empty;
            mediator
                .Setup(x => x.Send(
                    It.Is<EpaoPipelineStandardsRequest>(c => c.EpaoId.Equals(epaoId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaginatedList<EpaoPipelineStandardsResponse>(new List<EpaoPipelineStandardsResponse>(), 0, 1, pageSize));

            // Act

            var actual = await controller.GetEpaoPipelineStandards(epaoId, standardFilterId, providerFilterId, EPADateFilterId, orderBy, orderDirection, pageSize);

            // Assert

            actual.Should().NotBeNull();
            var actualResult = actual as BadRequestResult;
            actualResult.Should().NotBeNull();
        }


        [Test, MoqAutoData]
        public async Task Then_If_No_Pipeline_Data_Return_Ok_No_Data(
            string epaoId,
            string standardFilterId,
            string providerFilterId,
            string EPADateFilterId,
            string orderBy,
            string orderDirection,
            int pageSize,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] StandardQueryController controller)
        {
            // Arrange

            mediator
                .Setup(x => x.Send(
                    It.Is<EpaoPipelineStandardsRequest>(c => c.EpaoId.Equals(epaoId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaginatedList<EpaoPipelineStandardsResponse>(new List<EpaoPipelineStandardsResponse>(), 0, 1, pageSize));

            // Act

            var actual = await controller.GetEpaoPipelineStandards(epaoId, standardFilterId, providerFilterId, EPADateFilterId, orderBy, orderDirection, pageSize);

            // Assert

            actual.Should().NotBeNull();
            var actualResult = actual as OkObjectResult;
            actualResult.Should().NotBeNull();
            var actualModel = actualResult.Value as PaginatedList<EpaoPipelineStandardsResponse>;
            actualModel.TotalRecordCount.Should().Be(0);
        }


        [Test, MoqAutoData]
        public async Task Then_If_Pipeline_Data_Exists_Return_Ok_With_Paginated_Data(
            string epaoId,
            string standardFilterId,
            string providerFilterId,
            string EPADateFilterId,
            string orderBy,
            string orderDirection,
            int pageSize,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] StandardQueryController controller)
        {
            // Arrange

            mediator
                .Setup(x => x.Send(
                    It.Is<EpaoPipelineStandardsRequest>(c => c.EpaoId.Equals(epaoId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(validPaginatedResponse);

            // Act

            var actual = await controller.GetEpaoPipelineStandards(epaoId, standardFilterId, providerFilterId, EPADateFilterId, orderBy, orderDirection, pageSize);

            // Assert

            actual.Should().NotBeNull();
            var actualResult = actual as OkObjectResult;
            actualResult.Should().NotBeNull();
            var actualModel = actualResult.Value as PaginatedList<EpaoPipelineStandardsResponse>;
            actualModel.TotalRecordCount.Should().Be(14);
            actualModel.PageSize.Should().Be(10);
            actualModel.TotalPages.Should().Be(2);
            actualModel.Items.Should().NotBeNull();
            actualModel.Items.Should().HaveCount(14);
        }
    }
}

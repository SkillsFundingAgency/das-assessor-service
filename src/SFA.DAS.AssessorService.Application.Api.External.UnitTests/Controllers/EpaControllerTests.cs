﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.External.Controllers;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers
{
    public class EpaControllerTests
    {
        [Test, MoqAutoData]
        public async Task When_CreatingEpaRecord_CallsInternalApi_Then_ReturnEpaResponseWithResponseCode200(
            [Frozen] Mock<IHeaderInfo> headerInfo,
            [Frozen] Mock<IApiClient> apiClient,
            CreateEpaRequest request,
            IEnumerable<CreateEpaResponse> response,
            [Greedy] EpaController sut)
        {
            //Arrange
            apiClient.Setup(client => client.CreateEpas(It.Is<IEnumerable<CreateBatchEpaRequest>>(s =>
                s.First().UkPrn == headerInfo.Object.Ukprn &&
                s.First().RequestId == request.RequestId &&
                s.First().Learner == request.Learner &&
                s.First().LearningDetails == request.LearningDetails &&
                s.First().Standard == request.Standard &&
                s.First().EpaDetails == request.EpaDetails))).ReturnsAsync(response);

            //Act
            var result = await sut.CreateEpaRecords(new List<CreateEpaRequest> { request }) as ObjectResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = result.Value as IEnumerable<CreateEpaResponse>;

            model.Should().BeEquivalentTo(response);
        }

        [Test, MoqAutoData]
        public async Task When_CreatingEpaRecord_WithTooManyRequestsInBatch_CallsInternalApi_Then_ReturnApiResponseWithResponseCode403(
            CreateEpaRequest request,
            [Greedy] EpaController sut)
        {
            //Arrange
            var requests = new List<CreateEpaRequest>();
            for(var i = 0; i < 26; i++)
            {
                requests.Add(request);
            }

            //Act
            var result = await sut.CreateEpaRecords(requests) as ObjectResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            ((ApiResponse)result.Value).Message.Should().Be("Batch limited to 25 requests");
        }

        [Test, MoqAutoData]
        public async Task When_UpdatingEpaRecord_CallsInternalApi_Then_ReturnEpaResponseWithResponseCode200(
            [Frozen] Mock<IHeaderInfo> headerInfo,
            [Frozen] Mock<IApiClient> apiClient,
            UpdateEpaRequest request,
            IEnumerable<UpdateEpaResponse> response,
            [Greedy] EpaController sut)
        {
            //Arrange
            apiClient.Setup(client => client.UpdateEpas(It.Is<IEnumerable<UpdateBatchEpaRequest>>(s =>
                s.First().UkPrn == headerInfo.Object.Ukprn &&
                s.First().EpaReference == request.EpaReference &&
                s.First().RequestId == request.RequestId &&
                s.First().Learner == request.Learner &&
                s.First().LearningDetails == request.LearningDetails &&
                s.First().Standard == request.Standard &&
                s.First().EpaDetails == request.EpaDetails))).ReturnsAsync(response);

            //Act
            var result = await sut.UpdateEpaRecords(new List<UpdateEpaRequest> { request }) as ObjectResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = result.Value as IEnumerable<UpdateEpaResponse>;

            model.Should().BeEquivalentTo(response);
        }


        [Test, MoqAutoData]
        public async Task When_UpdatingEpaRecord_WithTooManyRequestsInBatch_CallsInternalApi_Then_ReturnApiResponseWithResponseCode403(
            UpdateEpaRequest request,
            [Greedy] EpaController sut)
        {
            //Arrange
            var requests = new List<UpdateEpaRequest>();
            for (var i = 0; i < 26; i++)
            {
                requests.Add(request);
            }

            //Act
            var result = await sut.UpdateEpaRecords(requests) as ObjectResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            ((ApiResponse)result.Value).Message.Should().Be("Batch limited to 25 requests");
        }

        [Test, MoqAutoData]
        public async Task When_DeletingEpaRecord_CallsInternalApi_Then_ReturnResponseCode204(
            [Frozen] Mock<IHeaderInfo> headerInfo,
            [Frozen] Mock<IApiClient> apiClient,
            long uln, string familyName, string standard, string epaReference,
            ApiResponse response,
            [Greedy] EpaController sut)
        {
            //Arrange
            response = null;
            apiClient.Setup(client => client.DeleteEpa(It.Is<DeleteBatchEpaRequest>(s =>
                s.UkPrn == headerInfo.Object.Ukprn &&
                s.EpaReference == epaReference && s.Uln == uln && s.FamilyName == familyName && s.Standard == standard))).ReturnsAsync(response);

            //Act
            var result = await sut.DeleteEpaRecord(uln, familyName, standard, epaReference) as NoContentResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }


        [Test, MoqAutoData]
        public async Task When_DeletingEpaRecord_CallsInternalApi_AndItErrors_Then_ReturnErrorResponseCode(
            [Frozen] Mock<IHeaderInfo> headerInfo,
            [Frozen] Mock<IApiClient> apiClient,
            long uln, string familyName, string standard, string epaReference,
            [Greedy] EpaController sut)
        {
            //Arrange
            var response = new ApiResponse((int)HttpStatusCode.BadRequest);
            
            apiClient.Setup(client => client.DeleteEpa(It.Is<DeleteBatchEpaRequest>(s =>
                s.UkPrn == headerInfo.Object.Ukprn &&
                s.EpaReference == epaReference && s.Uln == uln && s.FamilyName == familyName && s.Standard == standard))).ReturnsAsync(response);

            //Act
            var result = await sut.DeleteEpaRecord(uln, familyName, standard, epaReference) as ObjectResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}

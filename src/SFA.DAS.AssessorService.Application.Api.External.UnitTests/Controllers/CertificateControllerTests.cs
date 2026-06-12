using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.External.Controllers;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers
{
    public class CertificateControllerTests
    {
        private Fixture _fixture;
        private GetCertificateResponse _response;

        private Mock<IApiClient> _mockApiClient;
        private Mock<IHeaderInfo> _headerInfo;
        
        private CertificateController _controller;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _response = _fixture.Build<GetCertificateResponse>()
                .With(r => r.ValidationErrors, new List<string>())
                .Create();

            _mockApiClient = new Mock<IApiClient>();
            _headerInfo = new Mock<IHeaderInfo>();
            _headerInfo.SetupGet(s => s.Ukprn).Returns(_fixture.Create<int>());
            _headerInfo.SetupGet(s => s.Email).Returns(_fixture.Create<string>());

            _controller = new CertificateController(Mock.Of<ILogger<CertificateController>>(), _headerInfo.Object, _mockApiClient.Object);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_CertificateExists_Then_ReturnCertificateWithResponseCode200(long uln, string familyName, string standard)
        {
            _mockApiClient.Setup(client => client.GetCertificate(It.Is<GetBatchCertificateRequest>(r => r.FamilyName == familyName && r.Uln == uln && r.Standard == standard)))
                .ReturnsAsync(_response);
            
            var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = result.Value as Certificate;

            model.CertificateData.Should().BeEquivalentTo(_response.Certificate.CertificateData);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_CertificateDoesNotExist_Then_ReturnResponseCode204(long uln, string familyName, string standard)
        {
            _response.Certificate = null;

            _mockApiClient.Setup(client => client.GetCertificate(It.IsAny<GetBatchCertificateRequest>()))
                .ReturnsAsync(_response);

            var result = await _controller.GetCertificate(uln, familyName, standard) as NoContentResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_LearnerDoesNotExist_Then_ReturnResponseCode403(long uln, string familyName, string standard)
        {
            _response.ValidationErrors = new List<string> { "Validation Error" };

            _mockApiClient.Setup(client => client.GetCertificate(It.IsAny<GetBatchCertificateRequest>()))
                .ReturnsAsync(_response);

            var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_CertificateStatusIsPrinted_Then_DeliveryStatusIsWaitingForDelivery(long uln, string familyName, string standard)
        {
            _response.Certificate.Status.CurrentStatus = CertificateStatus.Printed;

            _mockApiClient.Setup(client => client.GetCertificate(It.IsAny<GetBatchCertificateRequest>()))
                .ReturnsAsync(_response);

            var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

            var certificate = result.Value as Certificate;

            certificate.Delivered.Status.Should().Be("WaitingForDelivery");
            certificate.Delivered.DeliveryDate.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task When_CreatingCertificateRecord_WithTooManyRequestsInBatch_CallsApi_Then_ReturnApiResponseWithResponseCode403()
        {
            //Arrange
            var fixture = new Fixture();
            var requests = fixture.CreateMany<CreateCertificateRequest>(26);
            
            //Act
            var result = await _controller.CreateCertificates(requests) as ObjectResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            ((ApiResponse)result.Value).Message.Should().Be("Batch limited to 25 requests");
        }

        [Test, MoqAutoData]
        public async Task When_CreatingCertificateRecord_CallsInternalApi_Then_ReturnCertificateWithResponseCode200(
            CreateCertificateRequest request,
            IEnumerable<CreateCertificateResponse> response)
        {
            //Arrange
            CreateBatchCertificateRequest transformedRequest = null;
            var requests = new List<CreateCertificateRequest> { request };
            _mockApiClient.Setup(s => s.CreateCertificates(It.IsAny<IEnumerable<CreateBatchCertificateRequest>>()))
                .Callback<IEnumerable<CreateBatchCertificateRequest>>((input) => transformedRequest = input.First())
                .ReturnsAsync(response);

            //Act
            var result = await _controller.CreateCertificates(requests) as ObjectResult;

            //Assert
            result.StatusCode.Value.Should().Be((int)HttpStatusCode.OK);
            transformedRequest.Should().NotBeNull();
            transformedRequest.Should().BeEquivalentTo(new
            {
                UkPrn = _headerInfo.Object.Ukprn,
                request.RequestId,
                CertificateData = new
                {
                    request.Standard,
                    request.Learner,
                    request.LearningDetails,
                    request.PostalContact
                }
            });

            result.Value.Should().Be(response);        
        }

        [Test, MoqAutoData]
        public async Task When_UpdatingCertificateRecord_WithTooManyRequestsInBatch_CallsApi_Then_ReturnApiResponseWithResponseCode403()
        {
            //Arrange
            var fixture = new Fixture();
            var requests = fixture.CreateMany<UpdateCertificateRequest>(26);

            //Act
            var result = await _controller.UpdateCertificates(requests) as ObjectResult;

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            ((ApiResponse)result.Value).Message.Should().Be("Batch limited to 25 requests");
        }

        [Test, MoqAutoData]
        public async Task When_UpdatingCertificateRecord_CallsInternalApi_Then_ReturnCertificateWithResponseCode200(
            UpdateCertificateRequest request,
            IEnumerable<UpdateCertificateResponse> response)
        {
            //Arrange
            UpdateBatchCertificateRequest transformedRequest = null;
            var requests = new List<UpdateCertificateRequest> { request };
            _mockApiClient.Setup(s => s.UpdateCertificates(It.IsAny<IEnumerable<UpdateBatchCertificateRequest>>()))
                .Callback<IEnumerable<UpdateBatchCertificateRequest>>((input) => transformedRequest = input.First())
                .ReturnsAsync(response);

            //Act
            var result = await _controller.UpdateCertificates(requests) as ObjectResult;

            //Assert
            result.StatusCode.Value.Should().Be((int)HttpStatusCode.OK);
            transformedRequest.Should().NotBeNull();
            transformedRequest.Should().BeEquivalentTo(new
            {
                UkPrn = _headerInfo.Object.Ukprn,
                request.RequestId,
                CertificateData = new
                {
                    request.Standard,
                    request.Learner,
                    request.LearningDetails,
                    request.PostalContact
                }
            });

            result.Value.Should().Be(response);
        }
    }
}

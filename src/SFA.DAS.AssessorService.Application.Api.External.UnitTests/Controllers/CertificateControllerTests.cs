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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers
{
    public class CertificateControllerTests
    {
        private Fixture _fixture;
        private GetCertificateResponse _certificateResponse;
        private GetCertificateLogsResponse _logsResponse;

        private Mock<IApiClient> _mockApiClient;
        private Mock<IHeaderInfo> _headerInfo;
        
        private CertificateController _controller;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _certificateResponse = _fixture.Build<GetCertificateResponse>()
                .With(r => r.ValidationErrors, new List<string>())
                .Create();

            _logsResponse = new GetCertificateLogsResponse { ValidationErrors = new List<string>()};

            _mockApiClient = new Mock<IApiClient>();
            _headerInfo = new Mock<IHeaderInfo>();
            _headerInfo.SetupGet(s => s.Ukprn).Returns(_fixture.Create<int>());
            _headerInfo.SetupGet(s => s.Email).Returns(_fixture.Create<string>());

            _mockApiClient.Setup(client => client.GetCertificate(It.IsAny<GetBatchCertificateRequest>()))
               .ReturnsAsync(_certificateResponse);

            _controller = new CertificateController(Mock.Of<ILogger<CertificateController>>(), _headerInfo.Object, _mockApiClient.Object);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_CertificateExists_Then_ReturnCertificateWithResponseCode200(long uln, string familyName, string standard)
        {
            _mockApiClient.Setup(client => client.GetCertificate(It.Is<GetBatchCertificateRequest>(r => r.FamilyName == familyName && r.Uln == uln && r.Standard == standard)))
                .ReturnsAsync(_certificateResponse);
            
            var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = result.Value as Certificate;

            model.CertificateData.Should().BeEquivalentTo(_certificateResponse.Certificate.CertificateData);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_CertificateDoesNotExist_Then_ReturnResponseCode204(long uln, string familyName, string standard)
        {
            _certificateResponse.Certificate = null;

            _mockApiClient.Setup(client => client.GetCertificate(It.IsAny<GetBatchCertificateRequest>()))
                .ReturnsAsync(_certificateResponse);

            var result = await _controller.GetCertificate(uln, familyName, standard) as NoContentResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_LearnerDoesNotExist_Then_ReturnResponseCode403(long uln, string familyName, string standard)
        {
            _certificateResponse.ValidationErrors = new List<string> { "Validation Error" };

            _mockApiClient.Setup(client => client.GetCertificate(It.IsAny<GetBatchCertificateRequest>()))
                .ReturnsAsync(_certificateResponse);

            var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingCertificate_And_CertificateStatusIsPrinted_Then_DeliveryStatusIsWaitingForDelivery(long uln, string familyName, string standard)
        {
            _certificateResponse.Certificate.Status.CurrentStatus = CertificateStatus.Printed;

            var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

            var certificate = result.Value as Certificate;

            certificate.Delivered.Status.Should().Be("WaitingForDelivery");
            certificate.Delivered.DeliveryDate.Should().BeNull();
        }

        //[Test, MoqAutoData]
        //public async Task When_RequestingCertificate_And_CertificateStatusIsNotDelivered_Then_DeliveryStatusIsNotDelivered(long uln, string familyName, string standard)
        //{
        //    _certificateResponse.Certificate.Status.CurrentStatus = CertificateStatus.NotDelivered;

        //    _logsResponse = _fixture.Build<GetCertificateLogsResponse>()
        //        .With(x => x.ValidationErrors, new List<string>())
        //        .With(x => x.CertificateLogs, new List<CertificateLog> {
        //                _fixture.Build<CertificateLog>()
        //                .With(x => x.Status, CertificateStatus.NotDelivered).Create()
        //        }).Create();

        //    _mockApiClient.Setup(client => client.GetCertificateLogs(It.IsAny<string>()))
        //        .ReturnsAsync(_logsResponse);

        //    var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

        //    var certificate = result.Value as Certificate;

        //    certificate.Delivered.Status.Should().Be(CertificateStatus.NotDelivered);
        //    certificate.Delivered.DeliveryDate.Should().Be(_logsResponse.CertificateLogs.First().EventTime);
        //}

        //[Test, MoqAutoData]
        //public async Task When_RequestingCertificate_And_LogsContainNotDeliveredAndDelivered_Then_DeliveryStatusIsDelivered(long uln, string familyName, string standard)
        //{
        //    _certificateResponse.Certificate.Status.CurrentStatus = CertificateStatus.Delivered;

        //    var deliveredLog = _fixture.Build<CertificateLog>()
        //        .With(x => x.Status, CertificateStatus.Delivered)
        //        .With(x => x.EventTime, DateTime.Now)
        //        .Create();

        //    _logsResponse = _fixture.Build<GetCertificateLogsResponse>()
        //        .With(x => x.ValidationErrors, new List<string>())
        //        .With(x => x.CertificateLogs, new List<CertificateLog> {
        //                _fixture.Build<CertificateLog>()
        //                    .With(x => x.Status, CertificateStatus.NotDelivered)
        //                    .With(x => x.EventTime, DateTime.Now.AddDays(-2))
        //                    .Create(),
        //                deliveredLog
        //        }).Create();

        //    _mockApiClient.Setup(client => client.GetCertificateLogs(It.IsAny<string>()))
        //        .ReturnsAsync(_logsResponse);

        //    var result = await _controller.GetCertificate(uln, familyName, standard) as ObjectResult;

        //    var certificate = result.Value as Certificate;

        //    certificate.Delivered.Status.Should().Be(CertificateStatus.Delivered);
        //    certificate.Delivered.DeliveryDate.Should().BeCloseTo(deliveredLog.EventTime);
        //}

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

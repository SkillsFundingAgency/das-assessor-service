
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
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers
{
    public class CertificateControllerTests
    {
        public  GetCertificateResponse _response;

        private Mock<IApiClient> _mockApiClient;

        private CertificateController _controller;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _response = fixture.Build<GetCertificateResponse>()
                .With(r => r.ValidationErrors, new List<string>())
                .Create();

            _mockApiClient = new Mock<IApiClient>();

            _controller = new CertificateController(Mock.Of<ILogger<CertificateController>>(), Mock.Of<IHeaderInfo>(), _mockApiClient.Object);
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
    }
}

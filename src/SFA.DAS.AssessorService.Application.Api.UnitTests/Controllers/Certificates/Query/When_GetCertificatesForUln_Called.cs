using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class When_GetCertificatesForUln_Called
    {
        private CertificateQueryController _sut;
        private Mock<IMediator> _mediatorMock;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new CertificateQueryController(_mediatorMock.Object);
        }

        [Test]
        public async Task Then_ReturnsListOfSummaries_WhenFound()
        {
            // Arrange
            var uln = 1234567890L;
            var expectedList = new GetCertificatesUlnResponse
            {
                Certificates = new List<ApprenticeCertificateSummary>
                {
                    new() { CertificateId = Guid.NewGuid(), CertificateType = "Standard", CourseCode = "123" },
                    new() { CertificateId = Guid.NewGuid(), CertificateType = "Standard", CourseCode = "456" }
                }
            };

            _mediatorMock
                .Setup(x => x.Send(It.Is<GetCertificatesUlnRequest>(r => r.Uln == uln),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _sut.GetCertificatesUln(uln) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().BeEquivalentTo(expectedList);
        }
    }
}

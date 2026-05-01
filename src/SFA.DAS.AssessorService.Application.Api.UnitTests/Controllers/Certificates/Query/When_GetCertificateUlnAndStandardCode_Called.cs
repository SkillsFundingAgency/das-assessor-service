using System;
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
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class When_GetCertificateUlnAndStandardCode_Called
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
        public async Task Then_ReturnsCertificate_WhenFound()
        {
            // Arrange
            var uln = 1234567890L;
            var standardCode = 99;
            var expectedCertificate = new Certificate { Id = Guid.NewGuid(), Uln = uln, StandardCode = standardCode };

            _mediatorMock
                .Setup(x => x.Send(It.Is<GetCertificateUlnAndStandardCodeRequest>(r =>
                        r.Uln == uln && r.StandardCode == standardCode),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCertificate);

            // Act
            var result = await _sut.GetCertificateUlnAndStandardCode(uln, standardCode) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().BeEquivalentTo(expectedCertificate);
        }
    }
}

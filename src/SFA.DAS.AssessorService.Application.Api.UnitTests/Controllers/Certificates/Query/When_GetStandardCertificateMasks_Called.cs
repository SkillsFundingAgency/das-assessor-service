using System.Collections.Generic;
using System.Linq;
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

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class When_GetStandardCertificateMasks_Called
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
        public async Task Then_Returns_Standard_Masks_When_Found()
        {
            var exclude = new long[] { 1111111111 };

            var expectedResponse = new GetStandardCertificateMasksResponse
            {
                Masks = new List<CertificateMask>
                {
                    new CertificateMask { CertificateType = "Standard", CourseCode = "100", CourseName = "Std Test" }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetStandardCertificateMasksRequest>(r => r.Exclude != null && r.Exclude.SequenceEqual(exclude)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _sut.GetStandardCertificateMasks(exclude) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().BeEquivalentTo(new { masks = expectedResponse.Masks });
        }

        [Test]
        public async Task Then_Returns_BadRequest_For_Invalid_Exclude_On_Standard()
        {
            var exclude = new long[] { -1 };

            var result = await _sut.GetStandardCertificateMasks(exclude) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(new { error = "Exclude ULN values must be positive integers." });
        }
    }
}

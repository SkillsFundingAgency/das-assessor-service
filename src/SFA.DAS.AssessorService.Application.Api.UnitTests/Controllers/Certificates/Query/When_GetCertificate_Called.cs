using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class When_GetCertificate_Called
    {
        private Standard _standardResponse;
        private Certificate _certificateResponse;

        private Mock<IMediator> _mockMediator;
        private Mock<IValidator<GetBatchCertificateRequest>> _mockGetValidator;
        private CertificateBatchController _controller;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _standardResponse = fixture.Create<Standard>();
            _certificateResponse = new Certificate
            {
                StandardCode = 1
            };
            
            _mockMediator = new Mock<IMediator>();
            _mockGetValidator = new Mock<IValidator<GetBatchCertificateRequest>>();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetStandardVersionRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_standardResponse);

            _mockMediator.Setup(m => m.Send(It.IsAny<GetBatchCertificateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_certificateResponse);

            _mockGetValidator.Setup(validator => validator.ValidateAsync(It.IsAny<GetBatchCertificateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _controller = new CertificateBatchController(_mockMediator.Object,
                _mockGetValidator.Object,
                Mock.Of<IValidator<CreateBatchCertificateRequest>>(),
                Mock.Of<IValidator<UpdateBatchCertificateRequest>> (),
                Mock.Of<IValidator<SubmitBatchCertificateRequest>>(),
                Mock.Of<IValidator<DeleteBatchCertificateRequest>>(),
                Mock.Of<IValidator<GetBatchCertificateLogsRequest>>());
        }

        [Test, MoqAutoData]
        public async Task Then_GetStandardVersionRequestIsSent(long uln, string lastname, string standardId, int ukprn)
        {
            await _controller.Get(uln, lastname, standardId, ukprn);

            _mockMediator.Verify(m => m.Send(It.Is<GetStandardVersionRequest>(r => r.StandardId == standardId && r.Version == null), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_GetCertificateQueryCalled(long uln, string lastname, string standardId, int ukprn)
        {
            await _controller.Get(uln, lastname, standardId, ukprn);

            _mockMediator.Verify(m => m.Send(It.Is<GetBatchCertificateRequest>(r => r.StandardCode == _standardResponse.LarsCode &&
                r.StandardReference == _standardResponse.IfateReferenceNumber &&
                r.Uln == uln &&
                r.UkPrn == ukprn &&
                r.FamilyName == lastname), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_RequestIsValid_Then_CertificateIsReturned(long uln, string lastname, string standardId, int ukprn)
        {
            var result = await _controller.Get(uln, lastname, standardId, ukprn) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = result.Value as GetBatchCertificateResponse;

            model.Certificate.Should().BeEquivalentTo(_certificateResponse);
        }

        [Test, MoqAutoData]
        public async Task And_RequestIsValid_And_CertificateIsNotFound_Then_ReturnNullCertificate(long uln, string lastname, string standardId, int ukprn)
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetBatchCertificateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Certificate)null);

            var result = await _controller.Get(uln, lastname, standardId, ukprn) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = result.Value as GetBatchCertificateResponse;

            model.Certificate.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task And_RequestIsInvalid_Then_GetCertificateQueryIsNotCalled(long uln, string lastname, string standardId, int ukprn)
        {
            SetupValidationError();

            await _controller.Get(uln, lastname, standardId, ukprn);

            _mockMediator.Verify(m => m.Send(It.IsAny<GetBatchCertificateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task And_RequestIsInvalid_Then_ValidationErrorsReturned(long uln, string lastname, string standardId, int ukprn)
        {
            SetupValidationError();

            var result = await _controller.Get(uln, lastname, standardId, ukprn) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = result.Value as GetBatchCertificateResponse;

            model.Certificate.Should().BeNull();
            model.ValidationErrors.First().Should().Be("Error message");
        }

        private void SetupValidationError()
        {
            _mockGetValidator.Setup(validator => validator.ValidateAsync(It.IsAny<GetBatchCertificateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure> {
                    new ValidationFailure("Error", "Error message")
                }));
        }
    }
}

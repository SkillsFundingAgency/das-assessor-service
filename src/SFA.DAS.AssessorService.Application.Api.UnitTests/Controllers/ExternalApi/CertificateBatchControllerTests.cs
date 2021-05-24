using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.ExternalApi
{
    public class CertificateBatchControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<IValidator<CreateBatchCertificateRequest>> _mockCreateBatchValidator;
        private Mock<IValidator<UpdateBatchCertificateRequest>> _mockUpdateBatchValidator;
        private Mock<IValidator<GetBatchCertificateRequest>> _mockGetBatchValidator;
        private Mock<IValidator<SubmitBatchCertificateRequest>> _mockSubmitBatchValidator;
        private Mock<IValidator<DeleteBatchCertificateRequest>> _mockDeleteBatchValidator;
        private CertificateBatchController _certificateBatchController;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();
            _mockCreateBatchValidator = new Mock<IValidator<CreateBatchCertificateRequest>>();
            _mockUpdateBatchValidator = new Mock<IValidator<UpdateBatchCertificateRequest>>();
            _mockGetBatchValidator  = new Mock<IValidator<GetBatchCertificateRequest>>();
            _mockSubmitBatchValidator = new Mock<IValidator<SubmitBatchCertificateRequest>>();
            _mockDeleteBatchValidator = new Mock<IValidator<DeleteBatchCertificateRequest>>();

            _certificateBatchController = new CertificateBatchController(_mockMediator.Object, 
                _mockGetBatchValidator.Object, _mockCreateBatchValidator.Object, 
                _mockUpdateBatchValidator.Object, _mockSubmitBatchValidator.Object, _mockDeleteBatchValidator.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingCreateCertificate_VersionIsSupplied_GetStandardVersionRequestSent_AndFieldsPopulated(
            Standard standard,
            CreateBatchCertificateRequest request)
        {
            //Arrange
            request.CertificateData.Version = "1.0";
            request.StandardCode = 0;
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _certificateBatchController.Create(new List<CreateBatchCertificateRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetStandardVersionRequest>(b => b.StandardId == request.StandardReference && b.Version == request.CertificateData.Version), new System.Threading.CancellationToken()), Times.Once);
            request.StandardCode.Should().Be(standard.LarsCode);
            request.StandardReference.Should().NotBe(standard.IfateReferenceNumber);
            request.StandardUId.Should().Be(standard.StandardUId);
            // Making sure the standard doesn't overwrite the version in populate fields.
            request.CertificateData.Version.Should().NotBe(standard.Version.VersionToString());
        }

        [Test, MoqAutoData]
        public async Task WhenCallingCreateCertificate_VersionIsNotSupplied_CalculateStandardVersionRequestSent_AndFieldsPopulated(
            Standard standard,
            CreateBatchCertificateRequest request)
        {
            //Arrange
            request.CertificateData.Version = null;
            request.StandardReference = null;
            _mockMediator.Setup(s => s.Send(It.IsAny<GetCalculatedStandardVersionForApprenticeshipRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _certificateBatchController.Create(new List<CreateBatchCertificateRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetCalculatedStandardVersionForApprenticeshipRequest>(b => b.StandardId == request.StandardCode.ToString() && b.Uln == request.Uln), new System.Threading.CancellationToken()), Times.Once);
            request.CertificateData.Version.Should().Be(standard.Version.VersionToString());
            request.StandardUId.Should().Be(standard.StandardUId);
            request.StandardReference.Should().Be(standard.IfateReferenceNumber);
            // Existing field shouldn't be overwritten.
            request.StandardCode.Should().NotBe(standard.LarsCode);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingCreateCertificate_ValidationNotSuccessful_ReturnsValidationErrors(
            Standard standard,
            IEnumerable<ValidationFailure> failures,
            CreateBatchCertificateRequest request)
        {
            //Arrange
            var validationResult = new ValidationResult(failures);
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(validationResult);

            //Act
            var controllerResult = await _certificateBatchController.Create(new List<CreateBatchCertificateRequest> { request }) as ObjectResult;

            //Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = controllerResult.Value as IEnumerable<BatchCertificateResponse>;
            var result = model.First();

            result.RequestId.Should().Be(request.RequestId);
            result.StandardCode.Should().Be(request.StandardCode);
            result.StandardReference.Should().Be(request.StandardReference);
            result.Uln.Should().Be(request.Uln);
            result.FamilyName.Should().Be(request.FamilyName);
            result.Certificate.Should().BeNull();
            result.ValidationErrors.Should().BeEquivalentTo(failures.Select(s => s.ErrorMessage));
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenCallingCreateCertificate_ValidationSuccessful_ReturnsCertificateDetails(
            Standard standard,
            Certificate certificate,
            CreateBatchCertificateRequest request)
        {
            //Arrange
            var validationResult = new ValidationResult();
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(s => s.Send(It.IsAny<CreateBatchCertificateRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(certificate);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(validationResult);

            //Act
            var controllerResult = await _certificateBatchController.Create(new List<CreateBatchCertificateRequest> { request }) as ObjectResult;

            //Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = controllerResult.Value as IEnumerable<BatchCertificateResponse>;
            var result = model.First();

            result.RequestId.Should().Be(request.RequestId);
            result.StandardCode.Should().Be(request.StandardCode);
            result.StandardReference.Should().Be(request.StandardReference);
            result.Uln.Should().Be(request.Uln);
            result.FamilyName.Should().Be(request.FamilyName);
            result.Certificate.Should().BeEquivalentTo(certificate);
            result.ValidationErrors.Should().BeEmpty();
        }
    }
}

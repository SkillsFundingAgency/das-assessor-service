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
    public class EpaBatchControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<IValidator<CreateBatchEpaRequest>> _mockCreateBatchValidator;
        private Mock<IValidator<UpdateBatchEpaRequest>> _mockUpdateBatchValidator;
        private Mock<IValidator<DeleteBatchEpaRequest>> _mockDeleteBatchValidator;
        private EpaBatchController _epaBatchController;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();
            _mockCreateBatchValidator = new Mock<IValidator<CreateBatchEpaRequest>>();
            _mockUpdateBatchValidator = new Mock<IValidator<UpdateBatchEpaRequest>>();
            _mockDeleteBatchValidator = new Mock<IValidator<DeleteBatchEpaRequest>>();

            _epaBatchController = new EpaBatchController(_mockMediator.Object, _mockCreateBatchValidator.Object, _mockUpdateBatchValidator.Object, _mockDeleteBatchValidator.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingCreateEpa_VersionIsSupplied_GetStandardVersionRequestSent_AndFieldsPopulated(
            Standard standard,
            CreateBatchEpaRequest request)
        {
            //Arrange
            request.Version = "1.0";
            request.StandardCode = 0;
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Create(new List<CreateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetStandardVersionRequest>(b => b.StandardId == request.StandardReference && b.Version == request.Version), new System.Threading.CancellationToken()), Times.Once);
            request.StandardCode.Should().Be(standard.LarsCode);
            request.StandardReference.Should().NotBe(standard.IfateReferenceNumber);
            request.StandardUId.Should().Be(standard.StandardUId);
            // Making sure the standard doesn't overwrite the version in populate fields.
            request.Version.Should().NotBe(standard.Version.VersionToString());
        }

        [Test, MoqAutoData]
        public async Task WhenCallingCreateEpa_VersionIsNotSupplied_CalculateStandardVersionRequestSent_AndFieldsPopulated(
            Standard standard,
            CreateBatchEpaRequest request)
        {
            //Arrange
            request.Version = null;
            request.StandardReference = null;
            _mockMediator.Setup(s => s.Send(It.IsAny<GetCalculatedStandardVersionForApprenticeshipRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Create(new List<CreateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetCalculatedStandardVersionForApprenticeshipRequest>(b => b.StandardId == request.StandardCode.ToString() && b.Uln == request.Uln), new System.Threading.CancellationToken()), Times.Once);
            request.Version.Should().Be(standard.Version.VersionToString());
            request.StandardUId.Should().Be(standard.StandardUId);
            request.StandardReference.Should().Be(standard.IfateReferenceNumber);
            // Existing field shouldn't be overwritten.
            request.StandardCode.Should().NotBe(standard.LarsCode);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingCreateEpa_ValidationNotSuccessful_ReturnsValidationErrors(
            Standard standard,
            IEnumerable<ValidationFailure> failures,
            CreateBatchEpaRequest request)
        {
            //Arrange
            var validationResult = new ValidationResult(failures);
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(validationResult);

            //Act
            var controllerResult = await _epaBatchController.Create(new List<CreateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = controllerResult.Value as IEnumerable<BatchEpaResponse>;
            var result = model.First();

            result.RequestId.Should().Be(request.RequestId);
            result.StandardCode.Should().Be(request.StandardCode);
            result.StandardReference.Should().Be(request.StandardReference);
            result.Uln.Should().Be(request.Uln);
            result.FamilyName.Should().Be(request.FamilyName);
            result.EpaDetails.Should().BeNull();
            result.ValidationErrors.Should().BeEquivalentTo(failures.Select(s => s.ErrorMessage));
        }

        [Test, MoqAutoData]
        public async Task WhenCallingCreateEpa_ValidationSuccessful_ReturnsEPADetails(
            Standard standard,
            EpaDetails epaDetails,
            CreateBatchEpaRequest request)
        {
            //Arrange
            var validationResult = new ValidationResult();
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(s => s.Send(It.IsAny<CreateBatchEpaRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(epaDetails);
            _mockCreateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(validationResult);

            //Act
            var controllerResult = await _epaBatchController.Create(new List<CreateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = controllerResult.Value as IEnumerable<BatchEpaResponse>;
            var result = model.First();

            result.RequestId.Should().Be(request.RequestId);
            result.StandardCode.Should().Be(request.StandardCode);
            result.StandardReference.Should().Be(request.StandardReference);
            result.Uln.Should().Be(request.Uln);
            result.FamilyName.Should().Be(request.FamilyName);
            result.EpaDetails.Should().BeEquivalentTo(epaDetails);
            result.ValidationErrors.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task WhenCallingUpdateEpa_NoExistingCertificate_VersionIsSupplied_GetStandardVersionRequestSent_AndFieldsPopulated(
         Standard standard,
         UpdateBatchEpaRequest request)
        {
            //Arrange
            request.Version = "1.0";
            request.StandardCode = 0;
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(t => t.Send(It.Is<GetCertificateForUlnRequest>(s => s.Uln == request.Uln && s.StandardCode == standard.LarsCode), new System.Threading.CancellationToken())).ReturnsAsync((Certificate)null);
            _mockUpdateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Update(new List<UpdateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetStandardVersionRequest>(b => b.StandardId == request.StandardReference && b.Version == request.Version), new System.Threading.CancellationToken()), Times.Once);
            request.StandardCode.Should().Be(standard.LarsCode);
            request.StandardReference.Should().NotBe(standard.IfateReferenceNumber);
            request.StandardUId.Should().Be(standard.StandardUId);
            // Making sure the standard doesn't overwrite the version in populate fields.
            request.Version.Should().NotBe(standard.Version.VersionToString());
        }

        [Test, MoqAutoData]
        public async Task WhenCallingUpdateEpa_NoExistingCertificate_VersionIsNotSupplied_CalculateStandardVersionRequestSent_AndFieldsPopulated(
            Standard standard,
            UpdateBatchEpaRequest request)
        {
            //Arrange
            request.Version = null;
            request.StandardReference = null;
            _mockMediator.Setup(s => s.Send(It.IsAny<GetCalculatedStandardVersionForApprenticeshipRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(t => t.Send(It.Is<GetCertificateForUlnRequest>(s => s.Uln == request.Uln && s.StandardCode == standard.LarsCode), new System.Threading.CancellationToken())).ReturnsAsync((Certificate)null);
            _mockUpdateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Update(new List<UpdateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetCalculatedStandardVersionForApprenticeshipRequest>(b => b.StandardId == request.StandardCode.ToString() && b.Uln == request.Uln), new System.Threading.CancellationToken()), Times.Once);
            request.Version.Should().Be(standard.Version.VersionToString());
            request.StandardUId.Should().Be(standard.StandardUId);
            request.StandardReference.Should().Be(standard.IfateReferenceNumber);
            // Existing field shouldn't be overwritten.
            request.StandardCode.Should().NotBe(standard.LarsCode);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenCallingUpdateEpa_WithExistingCertificate_VersionIsSupplied_FieldsPopulatedFromStandardVersion_NotCertificate(
             Standard standard,
             Certificate certificate,
             CertificateData certificateData,
             UpdateBatchEpaRequest request)
        {
            //Arrange
            request.Version = "1.0";
            request.StandardCode = 0;
            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(t => t.Send(It.Is<GetCertificateForUlnRequest>(s => s.Uln == request.Uln && s.StandardCode == standard.LarsCode), new System.Threading.CancellationToken())).ReturnsAsync(certificate);
            _mockUpdateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Update(new List<UpdateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetStandardVersionRequest>(b => b.StandardId == request.StandardReference && b.Version == request.Version), new System.Threading.CancellationToken()), Times.Once);
            request.StandardCode.Should().Be(standard.LarsCode);
            request.StandardReference.Should().NotBe(standard.IfateReferenceNumber);
            request.StandardUId.Should().Be(standard.StandardUId);
                        
            request.StandardUId.Should().NotBe(certificate.StandardUId);
            request.Version.Should().NotBe(certificateData.Version);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenCallingUpdateEpa_WithExistingCertificate_VersionIsNotSupplied_CertificateUsedToPopulateFields(
            Standard standard,
            Certificate certificate,
            CertificateData certificateData,
            UpdateBatchEpaRequest request)
        {
            //Arrange
            request.Version = null;
            request.StandardReference = null;
            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockMediator.Setup(s => s.Send(It.IsAny<GetCalculatedStandardVersionForApprenticeshipRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(t => t.Send(It.Is<GetCertificateForUlnRequest>(s => s.Uln == request.Uln && s.StandardCode == standard.LarsCode), new System.Threading.CancellationToken())).ReturnsAsync(certificate);
            _mockUpdateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Update(new List<UpdateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetCalculatedStandardVersionForApprenticeshipRequest>(b => b.StandardId == request.StandardCode.ToString() && b.Uln == request.Uln), new System.Threading.CancellationToken()), Times.Once);
            request.Version.Should().Be(certificateData.Version);
            request.StandardUId.Should().Be(certificate.StandardUId);
            request.StandardReference.Should().Be(standard.IfateReferenceNumber);
            request.StandardCode.Should().NotBe(standard.LarsCode);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingUpdateEpa_ValidationNotSuccessful_ReturnsValidationErrors(
            Standard standard,
            IEnumerable<ValidationFailure> failures,
            UpdateBatchEpaRequest request)
        {
            //Arrange
            var validationResult = new ValidationResult(failures);
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(t => t.Send(It.Is<GetCertificateForUlnRequest>(s => s.Uln == request.Uln && s.StandardCode == standard.LarsCode), new System.Threading.CancellationToken())).ReturnsAsync((Certificate)null);
            _mockUpdateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult(failures));

            //Act
            var controllerResult = await _epaBatchController.Update(new List<UpdateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = controllerResult.Value as IEnumerable<BatchEpaResponse>;
            var result = model.First();

            result.RequestId.Should().Be(request.RequestId);
            result.StandardCode.Should().Be(request.StandardCode);
            result.StandardReference.Should().Be(request.StandardReference);
            result.Uln.Should().Be(request.Uln);
            result.FamilyName.Should().Be(request.FamilyName);
            result.EpaDetails.Should().BeNull();
            result.ValidationErrors.Should().BeEquivalentTo(failures.Select(s => s.ErrorMessage));
        }

        [Test, MoqAutoData]
        public async Task WhenCallingUpdateEpa_ValidationSuccessful_ReturnsEPADetails(
            Standard standard,
            EpaDetails epaDetails,
            UpdateBatchEpaRequest request)
        {
            //Arrange
            var validationResult = new ValidationResult();
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockMediator.Setup(t => t.Send(It.Is<GetCertificateForUlnRequest>(s => s.Uln == request.Uln && s.StandardCode == standard.LarsCode), new System.Threading.CancellationToken())).ReturnsAsync((Certificate)null);
            _mockMediator.Setup(s => s.Send(It.IsAny<UpdateBatchEpaRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(epaDetails);
            _mockUpdateBatchValidator.Setup(s => s.ValidateAsync(request, new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Update(new List<UpdateBatchEpaRequest> { request }) as ObjectResult;

            //Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var model = controllerResult.Value as IEnumerable<BatchEpaResponse>;
            var result = model.First();

            result.RequestId.Should().Be(request.RequestId);
            result.StandardCode.Should().Be(request.StandardCode);
            result.StandardReference.Should().Be(request.StandardReference);
            result.Uln.Should().Be(request.Uln);
            result.FamilyName.Should().Be(request.FamilyName);
            result.EpaDetails.Should().BeEquivalentTo(epaDetails);
            result.ValidationErrors.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task WhenCallingDeleteEpa_WithValidationErrors_ReturnsForbiddenStatusCode(
            Standard standard,
            IEnumerable<ValidationFailure> failures,
            DeleteBatchEpaRequest request)
        {
            //Arrange
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockDeleteBatchValidator.Setup(s => s.ValidateAsync(It.IsAny<DeleteBatchEpaRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult(failures));

            //Act
            var controllerResult = await _epaBatchController.Delete(request.Uln, request.FamilyName, request.StandardReference, request.EpaReference, request.UkPrn) as ObjectResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetStandardVersionRequest>(b => b.StandardId == request.StandardReference && b.Version == null), new System.Threading.CancellationToken()), Times.Once);
            _mockMediator.Verify(a => a.Send(It.Is<DeleteBatchEpaRequest>(b =>
                b.StandardCode == standard.LarsCode &&
                b.StandardReference == standard.IfateReferenceNumber &&
                b.UkPrn == request.UkPrn &&
                b.Uln == request.Uln &&
                b.EpaReference == request.EpaReference &&
                b.FamilyName == request.FamilyName), new System.Threading.CancellationToken()), Times.Never);

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingDeleteEpa_EPANotFound_ReturnsNotFound(
            Standard standard,
            DeleteBatchEpaRequest request)
        {
            //Arrange
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockDeleteBatchValidator.Setup(s => s.ValidateAsync(It.IsAny<DeleteBatchEpaRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());
            _mockMediator.Setup(a => a.Send(It.IsAny<DeleteBatchEpaRequest>(), new System.Threading.CancellationToken())).Throws<NotFound>();

            //Act
            var controllerResult = await _epaBatchController.Delete(request.Uln, request.FamilyName, request.StandardReference, request.EpaReference, request.UkPrn) as NotFoundResult;

            //Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingDeleteEpa_WithNoValidationError_SendsDeleteRequest(
            Standard standard,
            DeleteBatchEpaRequest request)
        {
            //Arrange
            request.StandardCode = 0;
            _mockMediator.Setup(s => s.Send(It.IsAny<GetStandardVersionRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(standard);
            _mockDeleteBatchValidator.Setup(s => s.ValidateAsync(It.IsAny<DeleteBatchEpaRequest>(), new System.Threading.CancellationToken())).ReturnsAsync(new ValidationResult());

            //Act
            var controllerResult = await _epaBatchController.Delete(request.Uln, request.FamilyName, request.StandardReference, request.EpaReference, request.UkPrn) as NoContentResult;

            //Assert
            _mockMediator.Verify(a => a.Send(It.Is<GetStandardVersionRequest>(b => b.StandardId == request.StandardReference && b.Version == null), new System.Threading.CancellationToken()), Times.Once);
            _mockMediator.Verify(a => a.Send(It.Is<DeleteBatchEpaRequest>(b => 
                b.StandardCode == standard.LarsCode &&
                b.StandardReference == standard.IfateReferenceNumber &&
                b.UkPrn == request.UkPrn &&
                b.Uln == request.Uln &&
                b.EpaReference == request.EpaReference &&
                b.FamilyName == request.FamilyName), new System.Threading.CancellationToken()), Times.Once);

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}

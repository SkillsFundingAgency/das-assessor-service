using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.CreateBatchCertificate
{
    public class WhenHandlingCreateBatchCertificateRequest
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<ILearnerRepository> _learnerRepository;
        private Mock<IOrganisationQueryRepository> _organisationQueryRepository;
        private Mock<IContactQueryRepository> _contactQueryRepository;
        private Mock<ILogger<CreateBatchCertificateHandler>> _logger;
        private Mock<IStandardService> _standardService;
        private Mock<IProvidersRepository> _mockProvidersRepository;

        private CreateBatchCertificateHandler _sut;

        private readonly long Uln = 12345678L;
        private readonly int StdCode = 123;
        private readonly int UkPrn = 111;
        private readonly int LearnerUkprn = 222;
        private readonly string StandardUId = "ST0123_1.0";

        private CreateBatchCertificateRequest _request;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _learnerRepository = new Mock<ILearnerRepository>();
            _organisationQueryRepository =
                new Mock<IOrganisationQueryRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();
            _logger = new Mock<ILogger<CreateBatchCertificateHandler>>();
            _standardService = new Mock<IStandardService>();
            _mockProvidersRepository = new Mock<IProvidersRepository>();

            _learnerRepository
                .Setup(m => m.Get(Uln, StdCode))
                .ReturnsAsync(new Domain.Entities.Learner
                {
                    UkPrn = LearnerUkprn
                });

            _organisationQueryRepository
                .Setup(m => m.GetByUkPrn(UkPrn))
                .ReturnsAsync(new Organisation());

            _standardService
                .Setup(m => m.GetStandardVersionById(StandardUId, null))
                .ReturnsAsync(new Standard());

            _standardService
                .Setup(m => m.GetStandardOptionsByStandardId(StandardUId))
                .ReturnsAsync(new StandardOptions());

            _mockProvidersRepository
                .Setup(m => m.GetProvider(LearnerUkprn))
                .ReturnsAsync(new Provider
                {
                    Ukprn = UkPrn,
                    Name = "PROVIDER"
                });

            _request = new CreateBatchCertificateRequest
            {
                StandardCode = StdCode,
                StandardUId = StandardUId,
                Uln = Uln,
                UkPrn = UkPrn,
                CertificateData = new CertificateData()
            };

            _sut = new CreateBatchCertificateHandler(
                _certificateRepository.Object,
                _learnerRepository.Object,
                _organisationQueryRepository.Object,
                _contactQueryRepository.Object,
                _logger.Object,
                _standardService.Object,
                _mockProvidersRepository.Object);
        }

        [Test]
        public async Task AndCertificateAlreadyExistsThenReturnsCertificate()
        {
            // Arrange
            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StdCode))
                .ReturnsAsync(new Certificate
                {
                    ProviderUkPrn = UkPrn,
                    CertificateData = new CertificateData()
                });

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    It.IsAny<Certificate>(),
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Start,
                    true,
                    null));

            result.StandardUId.Should().Be(StandardUId);
            result.Status.Should().Be(CertificateStatus.Draft);
            result.ProviderUkPrn.Should().Be(UkPrn);
        }

        [Test]
        public async Task AndCertificateDoesNotExistThenReturnsCertificate()
        {
            // Arrange
            var id = Guid.NewGuid();

            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StdCode))
                .ReturnsAsync((Certificate)null);

            _certificateRepository
                .Setup(m => m.NewStandardCertificate(
                    It.Is<Certificate>(c =>
                        c.Uln == Uln &&
                        c.ProviderUkPrn == LearnerUkprn &&
                        c.StandardCode == StdCode &&
                        c.CreatedBy == ExternalApiConstants.ApiUserName &&
                        c.Status == CertificateStatus.Draft)))
                .ReturnsAsync(new Certificate
                {
                    Id = id,
                    ProviderUkPrn = LearnerUkprn
                });

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    It.IsAny<Certificate>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    true,
                    null),
                Times.Never);

            result.Id.Should().Be(id);
            result.ProviderUkPrn.Should().Be(LearnerUkprn);
        }

        [Test]
        public async Task AndOrganisationDoesNotExistThenReturnsCertificate()
        {
            // Arrange
            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StdCode))
                .ReturnsAsync(new Certificate
                {
                    ProviderUkPrn = UkPrn,
                    CertificateData = new CertificateData()
                });

            _mockProvidersRepository
                .Setup(m => m.GetProvider(UkPrn))
                .ReturnsAsync((Provider)null);

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.StandardUId.Should().Be(StandardUId);
            result.Status.Should().Be(CertificateStatus.Draft);
            result.ProviderUkPrn.Should().Be(UkPrn);
        }

        [Test]
        public async Task WhenCreatingNewCertificate_AndLearnerDateOfBirthIsNull_ThenCertificateDateOfBirthIsNull()
        {
            // Arrange
            _learnerRepository
                .Setup(x => x.Get(Uln, StdCode))
                .ReturnsAsync(new Domain.Entities.Learner
                {
                    UkPrn = LearnerUkprn,
                    DateOfBirth = null
                });

            _certificateRepository
                .Setup(x => x.GetCertificate(Uln, StdCode))
                .ReturnsAsync((Certificate)null);

            Certificate createdCertificate = null;

            _certificateRepository
                .Setup(x => x.NewStandardCertificate(
                    It.IsAny<Certificate>()))
                .Callback<Certificate>(
                    certificate => createdCertificate = certificate)
                .ReturnsAsync(new Certificate());

            // Act
            await _sut.Handle(_request, CancellationToken.None);

            // Assert
            createdCertificate.DateOfBirth.Should().BeNull();
        }

        [Test]
        public async Task WhenReusingCertificate_ThenDateOfBirthIsUpdatedFromLearner()
        {
            // Arrange
            var learnerDateOfBirth =
                new DateTime(2000, 2, 3, 0, 0, 0, DateTimeKind.Utc);

            var learner = new Domain.Entities.Learner
            {
                UkPrn = LearnerUkprn,
                DateOfBirth = learnerDateOfBirth
            };

            var existingCertificate = new Certificate
            {
                DateOfBirth =
                    new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ProviderUkPrn = UkPrn,
                CertificateData = new CertificateData()
            };

            _learnerRepository
                .Setup(x => x.Get(Uln, StdCode))
                .ReturnsAsync(learner);

            _certificateRepository
                .Setup(x => x.GetCertificate(Uln, StdCode))
                .ReturnsAsync(existingCertificate);

            _certificateRepository
                .Setup(x => x.UpdateStandardCertificate(
                    existingCertificate,
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Start,
                    true,
                    null))
                .ReturnsAsync(existingCertificate);

            // Act
            await _sut.Handle(_request, CancellationToken.None);

            // Assert
            existingCertificate.DateOfBirth.Should()
                .Be(learnerDateOfBirth);
        }

        [Test]
        public async Task WhenReusingCertificate_AndLearnerDateOfBirthIsNull_ThenCertificateDateOfBirthIsUpdatedToNull()
        {
            // Arrange
            var learner = new Domain.Entities.Learner
            {
                UkPrn = LearnerUkprn,
                DateOfBirth = null
            };

            var existingCertificate = new Certificate
            {
                DateOfBirth =
                    new DateTime(2000, 2, 3, 0, 0, 0, DateTimeKind.Utc),
                ProviderUkPrn = UkPrn,
                CertificateData = new CertificateData()
            };

            _learnerRepository
                .Setup(x => x.Get(Uln, StdCode))
                .ReturnsAsync(learner);

            _certificateRepository
                .Setup(x => x.GetCertificate(Uln, StdCode))
                .ReturnsAsync(existingCertificate);

            _certificateRepository
                .Setup(x => x.UpdateStandardCertificate(
                    It.IsAny<Certificate>(),
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Start,
                    true,
                    null))
                .ReturnsAsync(existingCertificate);

            // Act
            await _sut.Handle(_request, CancellationToken.None);

            // Assert
            existingCertificate.DateOfBirth.Should().BeNull();

            _certificateRepository.Verify(x =>
                x.UpdateStandardCertificate(
                    It.Is<Certificate>(certificate =>
                        certificate.DateOfBirth == null),
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Start,
                    true,
                    null),
                Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Incoming")]
        public async Task WhenCreatingNewCertificate_ThenIncomingDeliveryDetailsAreIgnored(
            string incomingDeliveryValue)
        {
            // Arrange
            SetRequestDeliveryDetails(incomingDeliveryValue);

            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StdCode))
                .ReturnsAsync((Certificate)null);

            Certificate createdCertificate = null;

            _certificateRepository
                .Setup(m => m.NewStandardCertificate(
                    It.IsAny<Certificate>()))
                .Callback<Certificate>(
                    certificate => createdCertificate = certificate)
                .ReturnsAsync((Certificate certificate) => certificate);

            var expectedDeliveryDetails = new CertificateData();

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            createdCertificate.Should().NotBeNull();

            AssertDeliveryDetails(
                createdCertificate.CertificateData,
                expectedDeliveryDetails);

            AssertDeliveryDetails(
                result.CertificateData,
                expectedDeliveryDetails);

            _certificateRepository.Verify(m =>
                m.NewStandardCertificate(It.IsAny<Certificate>()),
                Times.Once);

            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    It.IsAny<Certificate>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    true,
                    null),
                Times.Never);
        }

        [Test]
        public async Task WhenReusingDeletedCertificate_ThenStoredDeliveryDetailsArePreserved(
            [Values(
                CertificateSendTo.None,
                CertificateSendTo.Apprentice,
                CertificateSendTo.Employer)]
            CertificateSendTo storedSendTo,
            [Values(null, "", "Replacement")]
            string incomingDeliveryValue)
        {
            // Arrange
            var storedData = new CertificateData
            {
                SendTo = storedSendTo,
                ContactName = "Existing recipient",
                ContactOrganisation = "Existing organisation",
                Department = "Existing department",
                ContactAddLine1 = "1 Existing Road",
                ContactAddLine2 = "Existing building",
                ContactAddLine3 = "Existing district",
                ContactAddLine4 = "Existing city",
                ContactPostCode = "CV1 2WT"
            };

            var existingCertificate = new Certificate
            {
                Id = Guid.NewGuid(),
                CertificateReference = "1234567890-1",
                Status = CertificateStatus.Deleted,
                ProviderUkPrn = UkPrn,
                CertificateData = storedData
            };

            SetRequestDeliveryDetails(incomingDeliveryValue);

            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StdCode))
                .ReturnsAsync(existingCertificate);

            _certificateRepository
                .Setup(m => m.UpdateStandardCertificate(
                    existingCertificate,
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Start,
                    true,
                    null))
                .ReturnsAsync(existingCertificate);

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            AssertDeliveryDetails(
                existingCertificate.CertificateData,
                storedData);

            AssertDeliveryDetails(
                result.CertificateData,
                storedData);

            result.Id.Should().Be(existingCertificate.Id);
            result.CertificateReference.Should().Be("1234567890-1");
            result.Status.Should().Be(CertificateStatus.Draft);

            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    existingCertificate,
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Start,
                    true,
                    null),
                Times.Once);

            _certificateRepository.Verify(m =>
                m.NewStandardCertificate(It.IsAny<Certificate>()),
                Times.Never);
        }

        private void SetRequestDeliveryDetails(string value)
        {
            _request.CertificateData.SendTo = CertificateSendTo.Employer;
            _request.CertificateData.ContactName = value;
            _request.CertificateData.ContactOrganisation = value;
            _request.CertificateData.Department = value;
            _request.CertificateData.ContactAddLine1 = value;
            _request.CertificateData.ContactAddLine2 = value;
            _request.CertificateData.ContactAddLine3 = value;
            _request.CertificateData.ContactAddLine4 = value;
            _request.CertificateData.ContactPostCode = value;
        }

        private static void AssertDeliveryDetails(
            CertificateData actual,
            CertificateData expected)
        {
            actual.SendTo.Should().Be(expected.SendTo);
            actual.ContactName.Should().Be(expected.ContactName);
            actual.ContactOrganisation.Should()
                .Be(expected.ContactOrganisation);
            actual.Department.Should().Be(expected.Department);
            actual.ContactAddLine1.Should().Be(expected.ContactAddLine1);
            actual.ContactAddLine2.Should().Be(expected.ContactAddLine2);
            actual.ContactAddLine3.Should().Be(expected.ContactAddLine3);
            actual.ContactAddLine4.Should().Be(expected.ContactAddLine4);
            actual.ContactPostCode.Should().Be(expected.ContactPostCode);
        }
    }
}
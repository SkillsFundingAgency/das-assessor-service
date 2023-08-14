using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.StartCertificateHandlerTests
{
    public class StartCertificateHandlerTests
    {
        private Mock<ILogger<StartCertificateHandler>> _mockLogger;
        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Mock<ILearnerRepository> _mockLearnerRepository;
        private Mock<IProvidersRepository> _mockProvidersRepository;
        private Mock<IOrganisationQueryRepository> _mockOrganisationQueryRepository;
        private Mock<IStandardService> _mockStandardService;
        private Mock<ICertificateNameCapitalisationService> _mockCertificateNameCapitalisationService;
        private StartCertificateHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<StartCertificateHandler>>();
            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockLearnerRepository = new Mock<ILearnerRepository>();
            _mockProvidersRepository = new Mock<IProvidersRepository>();
            _mockOrganisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            _mockStandardService = new Mock<IStandardService>();
            _mockCertificateNameCapitalisationService = new Mock<ICertificateNameCapitalisationService>();

            _sut = new StartCertificateHandler(_mockCertificateRepository.Object, _mockLearnerRepository.Object, _mockProvidersRepository.Object,
                _mockOrganisationQueryRepository.Object, _mockLogger.Object, _mockStandardService.Object, _mockCertificateNameCapitalisationService.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_CreatesNewCertificate_WhenStandardUIdNotSet(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            Provider provider,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            learnerRecord.FundingModel = 81;
            SetupCertificateNameCapitalisationService(learnerRecord.GivenNames);
            SetupCertificateNameCapitalisationService(learnerRecord.FamilyName);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(learnerRecord.StdCode)).ReturnsAsync(standards);
            Certificate createdCertificate = null;

            _mockCertificateRepository.Setup(s => s.New(It.IsAny<Certificate>())).Callback<Certificate>((cert) =>
            {
                createdCertificate = cert;
            }).ReturnsAsync(stubCertificate);

            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            response.Should().BeEquivalentTo(stubCertificate);

            createdCertificate.Should().BeEquivalentTo(new
            {
                request.Uln,
                request.StandardCode,
                ProviderUkPrn = learnerRecord.UkPrn,
                OrganisationId = organisationRecord.Id,
                CreatedBy = request.Username,
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                learnerRecord.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = false
            });

            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            certData.Should().BeEquivalentTo(new
            {
                LearnerGivenNames = learnerRecord.GivenNames,
                LearnerFamilyName = learnerRecord.FamilyName,
                LearningStartDate = learnerRecord.LearnStartDate,
                FullName = $"{learnerRecord.GivenNames} {learnerRecord.FamilyName}",
                ProviderName = provider.Name,
                StandardName = standards.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First().Title
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_CreatesNewPrivatelyFundedCertificate_WhenFundingModelIs99(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            Provider provider,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            learnerRecord.FundingModel = 99;
            SetupCertificateNameCapitalisationService(learnerRecord.GivenNames);
            SetupCertificateNameCapitalisationService(learnerRecord.FamilyName);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(learnerRecord.StdCode)).ReturnsAsync(standards);
            Certificate createdCertificate = null;

            _mockCertificateRepository.Setup(s => s.New(It.IsAny<Certificate>())).Callback<Certificate>((cert) =>
            {
                createdCertificate = cert;
            }).ReturnsAsync(stubCertificate);

            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            response.Should().BeEquivalentTo(stubCertificate);

            createdCertificate.Should().BeEquivalentTo(new
            {
                request.Uln,
                request.StandardCode,
                ProviderUkPrn = learnerRecord.UkPrn,
                OrganisationId = organisationRecord.Id,
                CreatedBy = request.Username,
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                learnerRecord.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = true
            });

            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            certData.Should().BeEquivalentTo(new
            {
                LearnerGivenNames = learnerRecord.GivenNames,
                LearnerFamilyName = learnerRecord.FamilyName,
                LearningStartDate = learnerRecord.LearnStartDate,
                FullName = $"{learnerRecord.GivenNames} {learnerRecord.FamilyName}",
                ProviderName = provider.Name,
                StandardName = standards.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First().Title
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_WhenStandardUIdSet_ButVersionInvalid_ThrowsException(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            Provider provider,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            SetupCertificateNameCapitalisationService(learnerRecord.GivenNames);
            SetupCertificateNameCapitalisationService(learnerRecord.FamilyName);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync((Standard)null);

            // Act
            Func<Task> act = async () => { await _sut.Handle(request, new CancellationToken()); };

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"StandardUId:{request.StandardUId} not found, unable to populate certificate data");
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_StandardUIdSet_CreatesNewCertificate_WithVersionDetails(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            Provider provider,
            Certificate stubCertificate,
            Standard standard)
        {
            // Arrange
            learnerRecord.FundingModel = 81;
            SetupCertificateNameCapitalisationService(learnerRecord.GivenNames);
            SetupCertificateNameCapitalisationService(learnerRecord.FamilyName);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync(standard);
            Certificate createdCertificate = null;

            _mockCertificateRepository.Setup(s => s.New(It.IsAny<Certificate>())).Callback<Certificate>((cert) =>
            {
                createdCertificate = cert;
            }).ReturnsAsync(stubCertificate);

            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            response.Should().BeEquivalentTo(stubCertificate);

            createdCertificate.Should().BeEquivalentTo(new
            {
                request.Uln,
                request.StandardCode,
                ProviderUkPrn = learnerRecord.UkPrn,
                OrganisationId = organisationRecord.Id,
                CreatedBy = request.Username,
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                learnerRecord.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = false,
                StandardUId = standard.StandardUId
            });

            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            certData.Should().BeEquivalentTo(new
            {
                LearnerGivenNames = learnerRecord.GivenNames,
                LearnerFamilyName = learnerRecord.FamilyName,
                LearningStartDate = learnerRecord.LearnStartDate,
                FullName = $"{learnerRecord.GivenNames} {learnerRecord.FamilyName}",
                ProviderName = provider.Name,
                StandardName = standard.Title,
                StandardReference = standard.IfateReferenceNumber,
                StandardLevel = standard.Level,
                StandardPublicationDate = standard.VersionApprovedForDelivery,
                Version = standard.Version,
                CourseOption = request.CourseOption
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndThereIsExistingCertificate_NotDeleted_UpdatesCertificateForNewGrade(
            StartCertificateRequest request,
            Organisation organisationRecord,
            Certificate existingCertificate,
            CertificateData certificateData,
            Standard standard,
            Provider provider,
            Domain.Entities.Learner learnerRecord)
        {
            // Arrange
            SetupCertificateNameCapitalisationService(learnerRecord.GivenNames);
            SetupCertificateNameCapitalisationService(learnerRecord.FamilyName);
            existingCertificate.Status = CertificateStatus.Submitted;
            certificateData.OverallGrade = CertificateGrade.Fail;
            existingCertificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(existingCertificate);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync(standard);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);

            Certificate updatedCertficate = null;
            _mockCertificateRepository.Setup(s => s.Update(It.IsAny<Certificate>(), It.IsAny<string>(), It.IsAny<string>(), true, null))
                .Callback<Certificate, string, string, bool, string>((cert, username, action, updateLogs, reason) =>
                {
                    updatedCertficate = cert;
                }).ReturnsAsync(updatedCertficate);


            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            updatedCertficate.Status.Should().Be(CertificateStatus.Draft);
            _mockCertificateRepository.Verify(s => s.Update(existingCertificate, request.Username, CertificateActions.Restart, true, null), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndThereIsExistingCertificate_WhichIsDeleted_ResetCertificateData(
            StartCertificateRequest request,
            Organisation organisationRecord,
            Certificate existingCertificate,
            Domain.Entities.Learner learnerRecord,
            CertificateData certificateData,
            Standard standard,
            Provider provider,
            Certificate stubCertificate)
        {
            // Arrange
            learnerRecord.FundingModel = 81;
            SetupCertificateNameCapitalisationService(learnerRecord.GivenNames);
            SetupCertificateNameCapitalisationService(learnerRecord.FamilyName);
            existingCertificate.Status = CertificateStatus.Deleted;
            existingCertificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(existingCertificate);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync(standard);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);

            Certificate updatedCertficate = null;
            _mockCertificateRepository.Setup(s => s.Update(It.IsAny<Certificate>(), It.IsAny<string>(), It.IsAny<string>(), true, null))
                .Callback<Certificate, string, string, bool, string>((cert, username, action, updateLogs, reason) =>
                    {
                        updatedCertficate = cert;
                    }).ReturnsAsync(stubCertificate);

            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            updatedCertficate.IsPrivatelyFunded.Should().BeFalse();

            var responseCertData = JsonConvert.DeserializeObject<CertificateData>(updatedCertficate.CertificateData);
            responseCertData.LearnerGivenNames.Should().Be(learnerRecord.GivenNames);
            responseCertData.LearnerFamilyName.Should().Be(learnerRecord.FamilyName);
            responseCertData.LearningStartDate.Should().Be(learnerRecord.LearnStartDate);
            responseCertData.FullName.Should().Be($"{learnerRecord.GivenNames} {learnerRecord.FamilyName}");

            _mockCertificateRepository.Verify(s => s.Update(existingCertificate, request.Username, null, true, null), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNamesAreInLowerCase_ThenRunProperCaseAlgorithm(StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            Provider provider,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            learnerRecord.FundingModel = 81;
            learnerRecord.GivenNames = learnerRecord.GivenNames.ToLower();
            learnerRecord.FamilyName = learnerRecord.FamilyName.ToLower();
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(learnerRecord.StdCode)).ReturnsAsync(standards);
            Certificate createdCertificate = null;

            _mockCertificateRepository.Setup(s => s.New(It.IsAny<Certificate>())).Callback<Certificate>((cert) =>
            {
                createdCertificate = cert;
            }).ReturnsAsync(stubCertificate);


            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            Assert.Multiple(() =>
            {
                _mockCertificateNameCapitalisationService.Verify(c => c.ProperCase(learnerRecord.GivenNames, false), Times.Once);
                _mockCertificateNameCapitalisationService.Verify(c => c.ProperCase(learnerRecord.FamilyName, true), Times.Once);
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNamesAreInUpperCase_ThenRunProperCaseAlgorithm(StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            Provider provider,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            learnerRecord.FundingModel = 81;
            learnerRecord.GivenNames = learnerRecord.GivenNames.ToUpper();
            learnerRecord.FamilyName = learnerRecord.FamilyName.ToUpper();
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(learnerRecord.StdCode)).ReturnsAsync(standards);
            Certificate createdCertificate = null;

            _mockCertificateRepository.Setup(s => s.New(It.IsAny<Certificate>())).Callback<Certificate>((cert) =>
            {
                createdCertificate = cert;
            }).ReturnsAsync(stubCertificate);


            // Act
            var response = await _sut.Handle(request, new CancellationToken());
            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            // Assertions
            Assert.Multiple(() =>
            {
                _mockCertificateNameCapitalisationService.Verify(c => c.ProperCase(learnerRecord.GivenNames, false), Times.Once);
                _mockCertificateNameCapitalisationService.Verify(c => c.ProperCase(learnerRecord.FamilyName, true), Times.Once);
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNamesAreInMixedCase_ThenDoNotRunProperCaseAlgorithm(StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            Provider provider,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            learnerRecord.FundingModel = 81;
            SetupCertificateNameCapitalisationService(learnerRecord.GivenNames);
            SetupCertificateNameCapitalisationService(learnerRecord.FamilyName);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockProvidersRepository.Setup(s => s.GetProvider(learnerRecord.UkPrn)).ReturnsAsync(provider);
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(learnerRecord.StdCode)).ReturnsAsync(standards);
            Certificate createdCertificate = null;

            _mockCertificateRepository.Setup(s => s.New(It.IsAny<Certificate>())).Callback<Certificate>((cert) =>
            {
                createdCertificate = cert;
            }).ReturnsAsync(stubCertificate);

            // Act
            var response = await _sut.Handle(request, new CancellationToken());
            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            // Assertions
            Assert.Multiple(() =>
            {
                _mockCertificateNameCapitalisationService.Verify(c => c.ProperCase(learnerRecord.GivenNames, It.IsAny<bool>()), Times.Never);
                _mockCertificateNameCapitalisationService.Verify(c => c.ProperCase(learnerRecord.FamilyName, true), Times.Never);
            });
        }

        private void SetupCertificateNameCapitalisationService(string name)
        {
            _mockCertificateNameCapitalisationService.Setup(s => s.ProperCase(It.IsAny<string>(), true)).Returns(name);
        }
    }
}

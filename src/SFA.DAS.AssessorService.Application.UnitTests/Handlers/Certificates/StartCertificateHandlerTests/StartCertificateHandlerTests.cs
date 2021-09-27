using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ProviderRegister;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.StartCertificateHandlerTests
{
    public class StartCertificateHandlerTests
    {
        private Mock<ILogger<StartCertificateHandler>> _mockLogger;
        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Mock<ILearnerRepository> _mockLearnerRepository;
        private Mock<IRoatpApiClient> _mockRoatpApiClient;
        private Mock<IOrganisationQueryRepository> _mockOrganisationQueryRepository;
        private Mock<IStandardService> _mockStandardService;
        private StartCertificateHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<StartCertificateHandler>>();
            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockLearnerRepository = new Mock<ILearnerRepository>();
            _mockRoatpApiClient = new Mock<IRoatpApiClient>();
            _mockOrganisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            _mockStandardService = new Mock<IStandardService>();

            _sut = new StartCertificateHandler(_mockCertificateRepository.Object, _mockLearnerRepository.Object, _mockRoatpApiClient.Object,
                _mockOrganisationQueryRepository.Object, _mockLogger.Object, _mockStandardService.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_CreatesNewCertificate_WhenStandardUIdNotSet(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            learnerRecord.FundingModel = 81;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(learnerRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
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
                ProviderName = organisationSearchResult.ProviderName,
                StandardName = standards.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First().Title
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_CreatesNewPrivatelyFundedCertificate_WhenFundingModelIs99(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            learnerRecord.FundingModel = 99;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(learnerRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
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
                ProviderName = organisationSearchResult.ProviderName,
                StandardName = standards.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First().Title
            });
        }

        [Test, RecursiveMoqAutoData]
        public void WhenHandlingStartCertificateRequest_AndNoExistingCertificate_WhenStandardUIdSet_ButVersionInvalid_ThrowsException(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(learnerRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync((Standard)null);

            // Act
            Func<Task> act = async () => { await _sut.Handle(request, new CancellationToken()); };

            act.Should().Throw<InvalidOperationException>().WithMessage("StandardUId Provided not recognised, unable to populate certificate data");
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_StandardUIdSet_CreatesNewCertificate_WithVersionDetails(
            StartCertificateRequest request,
            Domain.Entities.Learner learnerRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            Standard standard)
        {
            // Arrange
            learnerRecord.FundingModel = 81;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(learnerRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
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
                ProviderName = organisationSearchResult.ProviderName,
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
            OrganisationSearchResult organisationSearchResult,
            Domain.Entities.Learner learnerRecord)
        {
            // Arrange
            existingCertificate.Status = CertificateStatus.Submitted;
            certificateData.OverallGrade = CertificateGrade.Fail;
            existingCertificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(existingCertificate);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync(standard);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(learnerRecord.UkPrn)).ReturnsAsync(organisationSearchResult);

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
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate)
        {
            // Arrange
            learnerRecord.FundingModel = 81;
            existingCertificate.Status = CertificateStatus.Deleted;
            existingCertificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(existingCertificate);
            _mockLearnerRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(learnerRecord);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync(standard);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(learnerRecord.UkPrn)).ReturnsAsync(organisationSearchResult);

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
    }
}

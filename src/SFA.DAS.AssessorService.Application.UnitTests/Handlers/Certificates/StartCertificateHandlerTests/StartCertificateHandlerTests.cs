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
        private Mock<IIlrRepository> _mockIlrRepository;
        private Mock<IRoatpApiClient> _mockRoatpApiClient;
        private Mock<IOrganisationQueryRepository> _mockOrganisationQueryRepository;
        private Mock<IStandardService> _mockStandardService;
        private StartCertificateHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<StartCertificateHandler>>();
            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockIlrRepository = new Mock<IIlrRepository>();
            _mockRoatpApiClient = new Mock<IRoatpApiClient>();
            _mockOrganisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            _mockStandardService = new Mock<IStandardService>();

            _sut = new StartCertificateHandler(_mockCertificateRepository.Object, _mockIlrRepository.Object, _mockRoatpApiClient.Object,
                _mockOrganisationQueryRepository.Object, _mockLogger.Object, _mockStandardService.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_CreatesNewCertificate_WhenStandardUIdNotSet(
            StartCertificateRequest request,
            Ilr ilrRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            ilrRecord.FundingModel = 81;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockIlrRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(ilrRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(ilrRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(ilrRecord.StdCode)).ReturnsAsync(standards);
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
                ProviderUkPrn = ilrRecord.UkPrn,
                OrganisationId = organisationRecord.Id,
                CreatedBy = request.Username,
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                ilrRecord.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = false
            });

            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            certData.Should().BeEquivalentTo(new
            {
                LearnerGivenNames = ilrRecord.GivenNames,
                LearnerFamilyName = ilrRecord.FamilyName,
                LearningStartDate = ilrRecord.LearnStartDate,
                FullName = $"{ilrRecord.GivenNames} {ilrRecord.FamilyName}",
                ProviderName = organisationSearchResult.ProviderName,
                StandardName = standards.OrderByDescending(s => s.Version).First().Title
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_CreatesNewPrivatelyFundedCertificate_WhenFundingModelIs99(
            StartCertificateRequest request,
            Ilr ilrRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            request.StandardUId = null;
            ilrRecord.FundingModel = 99;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockIlrRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(ilrRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(ilrRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(ilrRecord.StdCode)).ReturnsAsync(standards);
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
                ProviderUkPrn = ilrRecord.UkPrn,
                OrganisationId = organisationRecord.Id,
                CreatedBy = request.Username,
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                ilrRecord.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = true
            });

            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            certData.Should().BeEquivalentTo(new
            {
                LearnerGivenNames = ilrRecord.GivenNames,
                LearnerFamilyName = ilrRecord.FamilyName,
                LearningStartDate = ilrRecord.LearnStartDate,
                FullName = $"{ilrRecord.GivenNames} {ilrRecord.FamilyName}",
                ProviderName = organisationSearchResult.ProviderName,
                StandardName = standards.OrderByDescending(s => s.Version).First().Title
            });
        }

        [Test, RecursiveMoqAutoData]
        public void WhenHandlingStartCertificateRequest_AndNoExistingCertificate_WhenStandardUIdSet_ButVersionInvalid_ThrowsException(
            StartCertificateRequest request,
            Ilr ilrRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            IEnumerable<Standard> standards)
        {
            // Arrange
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockIlrRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(ilrRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(ilrRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
            _mockStandardService.Setup(s => s.GetStandardVersionById(request.StandardUId, null)).ReturnsAsync((Standard)null);

            // Act
            Func<Task> act = async () => { await _sut.Handle(request, new CancellationToken()); };

            act.Should().Throw<InvalidOperationException>().WithMessage("StandardUId Provided not recognised, unable to start certificate request");
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndNoExistingCertificate_StandardUIdSet_CreatesNewCertificate_WithVersionDetails(
            StartCertificateRequest request,
            Ilr ilrRecord,
            Organisation organisationRecord,
            OrganisationSearchResult organisationSearchResult,
            Certificate stubCertificate,
            Standard standard)
        {
            // Arrange
            ilrRecord.FundingModel = 81;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockIlrRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(ilrRecord);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockRoatpApiClient.Setup(s => s.GetOrganisationByUkprn(ilrRecord.UkPrn)).ReturnsAsync(organisationSearchResult);
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
                ProviderUkPrn = ilrRecord.UkPrn,
                OrganisationId = organisationRecord.Id,
                CreatedBy = request.Username,
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                ilrRecord.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = false,
                StandardUId = standard.StandardUId
            });

            var certData = JsonConvert.DeserializeObject<CertificateData>(createdCertificate.CertificateData);

            certData.Should().BeEquivalentTo(new
            {
                LearnerGivenNames = ilrRecord.GivenNames,
                LearnerFamilyName = ilrRecord.FamilyName,
                LearningStartDate = ilrRecord.LearnStartDate,
                FullName = $"{ilrRecord.GivenNames} {ilrRecord.FamilyName}",
                ProviderName = organisationSearchResult.ProviderName,
                StandardName = standard.Title,
                StandardReference = standard.IfateReferenceNumber,
                StandardLevel = standard.Level,
                StandardPublicationDate = standard.EffectiveFrom,
                Version = standard.Version,
                CourseOption = request.CourseOption
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndThereIsExistingCertificate_NotDeleted_UpdatesCertificateForNewGrade(
            StartCertificateRequest request,
            Organisation organisationRecord,
            Certificate existingCertificate,
            CertificateData certificateData)
        {
            // Arrange
            existingCertificate.Status = CertificateStatus.Submitted;
            certificateData.OverallGrade = CertificateGrade.Fail;
            existingCertificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(existingCertificate);

            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            response.Status.Should().Be(CertificateStatus.Draft);
            _mockCertificateRepository.Verify(s => s.Update(existingCertificate, request.Username, CertificateActions.Restart, true, null), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenHandlingStartCertificateRequest_AndThereIsExistingCertificate_WhichIsDeleted_ResetCertificateData(
            StartCertificateRequest request,
            Organisation organisationRecord,
            Certificate existingCertificate,
            Ilr ilrRecord,
            CertificateData certificateData)
        {
            // Arrange
            ilrRecord.FundingModel = 81;
            existingCertificate.Status = CertificateStatus.Deleted;
            existingCertificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockOrganisationQueryRepository.Setup(s => s.GetByUkPrn(request.UkPrn)).ReturnsAsync(organisationRecord);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(existingCertificate);
            _mockIlrRepository.Setup(s => s.Get(request.Uln, request.StandardCode)).ReturnsAsync(ilrRecord);

            // Act
            var response = await _sut.Handle(request, new CancellationToken());

            // Assertions
            response.IsPrivatelyFunded.Should().BeFalse();

            var responseCertData = JsonConvert.DeserializeObject<CertificateData>(response.CertificateData);
            responseCertData.LearnerGivenNames.Should().Be(ilrRecord.GivenNames);
            responseCertData.LearnerFamilyName.Should().Be(ilrRecord.FamilyName);
            responseCertData.LearningStartDate.Should().Be(ilrRecord.LearnStartDate);
            responseCertData.FullName.Should().Be($"{ilrRecord.GivenNames} {ilrRecord.FamilyName}");

            _mockCertificateRepository.Verify(s => s.Update(existingCertificate, request.Username, null, true, null), Times.Once);
        }
    }
}

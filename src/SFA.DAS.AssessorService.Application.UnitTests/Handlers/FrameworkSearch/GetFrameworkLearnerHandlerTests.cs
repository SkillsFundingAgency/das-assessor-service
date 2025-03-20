using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Data.Interfaces;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using System;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using AutoFixture;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.FrameworkSearch
{
    [TestFixture]
    public class GetFrameworkLearnerHandlerTests : MapperBase
    {
        private GetFrameworkLearnerHandler _handler;
        private Mock<IFrameworkLearnerRepository> _frameworkLearnerRepository;
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IStaffCertificateRepository> _staffCertificateRepository;
        private Mock<IMapper> _mapperMock;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _frameworkLearnerRepository = new Mock<IFrameworkLearnerRepository>();
            _certificateRepository = new Mock<ICertificateRepository>();
            _staffCertificateRepository = new Mock<IStaffCertificateRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetFrameworkLearnerHandler(_mapperMock.Object, 
                _frameworkLearnerRepository.Object, _certificateRepository.Object, _staffCertificateRepository.Object);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

  
        }

        [Test, MoqAutoData]
        public async Task ThenRequestSentToFrameworkLearnerRepository(GetFrameworkLearnerRequest query, FrameworkLearner learner)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync(learner);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            _frameworkLearnerRepository.Verify(r => r.GetFrameworkLearner(query.Id), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task ThenMapperIsCalled(GetFrameworkLearnerRequest query, FrameworkLearner learner)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync(learner);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            _mapperMock.Verify(m => m.Map<GetFrameworkLearnerResponse>(learner), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task AndTheFrameworkLearnerIsNullThenDefaultIsReturned(GetFrameworkLearnerRequest query)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync((FrameworkLearner)null);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.Should().Be(default);
        }

        [Test, MoqAutoData]
        public async Task ThenFrameworkLearnerIsReturnedAsTheCorrectType(
            GetFrameworkLearnerRequest query, 
            FrameworkLearner learner,
            GetFrameworkLearnerResponse frameworkResult)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync(learner);

            _mapperMock
                .Setup(m => m.Map<GetFrameworkLearnerResponse>(learner))
                .Returns(frameworkResult);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.Should().BeOfType<GetFrameworkLearnerResponse>();
            result.Should().BeEquivalentTo(frameworkResult);
        }

        [Test, MoqAutoData]
        public async Task AndCertificateIsNullThenCertificateFieldsAreNotSet(
            GetFrameworkLearnerRequest query, 
            FrameworkLearner learner,
            GetFrameworkLearnerResponse frameworkResult)
        {
            // Arrange
            frameworkResult.CertificateReference = null;
            frameworkResult.CertificateStatus = null;
            frameworkResult.CertificateStatusDate = null;
            frameworkResult.CertificateLogs = null;

            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
            .ReturnsAsync(learner);

            _certificateRepository
                .Setup(r => r.GetFrameworkCertificate(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync((FrameworkCertificate)null);

            _mapperMock
                .Setup(m => m.Map<GetFrameworkLearnerResponse>(learner))
                .Returns(frameworkResult);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.CertificateLogs.Should().BeNull();
            result.CertificateReference.Should().BeNull();
            result.CertificateStatus.Should().BeNull();
            result.CertificateStatusDate.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task AndCertificateIsNotNullThenCertificateFieldsAreSet()
        {
            var query = _fixture.Create<GetFrameworkLearnerRequest>();
            var learner = _fixture.Create<FrameworkLearner>();
            var certificate = _fixture.Create<FrameworkCertificate>();
            var frameworkResult = _fixture.Create<GetFrameworkLearnerResponse>();
            
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync(learner);

             _certificateRepository
                .Setup(r => r.GetFrameworkCertificate(query.Id, It.IsAny<bool>()))
                .ReturnsAsync(certificate);

            _mapperMock
                .Setup(m => m.Map<GetFrameworkLearnerResponse>(learner))
                .Returns(frameworkResult);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.CertificateReference.Should().Be(certificate.CertificateReference);
            result.CertificateStatus.Should().Be(certificate.Status);
            result.CertificateStatusDate.Should().Be(certificate.UpdatedAt);
        }

        [Test, MoqAutoData]
        public async Task AndAllLogsRequestedThenAllLogsAreReturned()
        {
            // Arrange
            var query = _fixture.Create<GetFrameworkLearnerRequest>();
            var learner = _fixture.Create<FrameworkLearner>();
            var certificate = _fixture.Create<FrameworkCertificate>();
            var frameworkResult = _fixture.Create<GetFrameworkLearnerResponse>();
            List<CertificateLogSummary> allLogs = CreateCertificateLogs();
            query.AllLogs = true;

            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync(learner);

            _certificateRepository
                .Setup(r => r.GetFrameworkCertificate(query.Id, query.AllLogs))
                .ReturnsAsync(certificate);

            _staffCertificateRepository
                .Setup(r => r.GetAllCertificateLogs(certificate.Id))
                .ReturnsAsync(allLogs);

            _mapperMock
                .Setup(m => m.Map<GetFrameworkLearnerResponse>(learner))
                .Returns(frameworkResult);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.CertificateLogs.Should().BeEquivalentTo(allLogs);
        }

        [Test, MoqAutoData]
        public async Task AndLatestLogsRequestedThenLatestLogsAreReturned()
        {
            // Arrange
            var query = _fixture.Create<GetFrameworkLearnerRequest>();
            var learner = _fixture.Create<FrameworkLearner>();
            var certificate = _fixture.Create<FrameworkCertificate>();
            var frameworkResult = _fixture.Create<GetFrameworkLearnerResponse>();
            var latestLogs = CreateCertificateLogs();

            query.AllLogs = false;

            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync(learner);

            _certificateRepository
                .Setup(r => r.GetFrameworkCertificate(query.Id, query.AllLogs))
                .ReturnsAsync(certificate);

            _staffCertificateRepository
                .Setup(r => r.GetLatestCertificateLogs(certificate.Id, 3))
                .ReturnsAsync(latestLogs);

            _mapperMock
                .Setup(m => m.Map<GetFrameworkLearnerResponse>(learner))
                .Returns(frameworkResult);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.CertificateLogs.Should().BeEquivalentTo(latestLogs);
        }

        [Test]
        public async Task AndLogsCountGreaterThanOneThenDifferencesAreCalculated()
        {
            // Arrange
            var logs = CreateCertificateLogs();

            var query = _fixture.Create<GetFrameworkLearnerRequest>();
            var learner = _fixture.Create<FrameworkLearner>();
            var certificate = _fixture.Create<FrameworkCertificate>();
            var frameworkResult = _fixture.Create<GetFrameworkLearnerResponse>();

            _frameworkLearnerRepository
                .Setup(r => r.GetFrameworkLearner(query.Id))
                .ReturnsAsync(learner);

            _certificateRepository
                .Setup(r => r.GetFrameworkCertificate(query.Id, query.AllLogs))
                .ReturnsAsync(certificate);

            _staffCertificateRepository
                .Setup(r => r.GetLatestCertificateLogs(certificate.Id, 3))
                .ReturnsAsync(logs);

            _mapperMock
                .Setup(m => m.Map<GetFrameworkLearnerResponse>(learner))
                .Returns(frameworkResult);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            logs.Should().NotBeNullOrEmpty();

            // Check that one log has no differences
            logs.Count(log => log.DifferencesToPrevious.Count == 0).Should().Be(1);

            // Check that two logs have more than one difference
            logs.Count(log => log.DifferencesToPrevious.Count > 1).Should().Be(2);
        }

        private static List<CertificateLogSummary> CreateCertificateLogs()
        {
            return new List<CertificateLogSummary>()
            {
                new CertificateLogSummary
                {
                    EventTime = DateTime.Now.AddDays(-3),
                    Action = "Created",
                    ActionBy = "User1",
                    ActionByEmail = "user1@example.com",
                    Status = "Active",
                    CertificateData = JsonConvert.SerializeObject(new CertificateData
                    {
                        LearnerGivenNames = "John",
                        LearnerFamilyName = "Doe",
                        EmployerName = "Company A",
                        StandardReference = "Ref1",
                        Version = "1.0",
                        IncidentNumber = "123"
                    }),
                    BatchNumber = 1,
                    ReasonForChange = "Initial creation"
                },
                new CertificateLogSummary
                {
                    EventTime = DateTime.Now.AddDays(-2),
                    Action = "Updated",
                    ActionBy = "User2",
                    ActionByEmail = "user2@example.com",
                    Status = "Active",
                    CertificateData = JsonConvert.SerializeObject(new CertificateData
                    {
                        LearnerGivenNames = "Jane",
                        LearnerFamilyName = "Smith",
                        EmployerName = "Company B",
                        StandardReference = "Ref2",
                        Version = "1.1",
                        IncidentNumber = "123"
                    }),
                    BatchNumber = 2,
                    ReasonForChange = "Updated data"
                },
                new CertificateLogSummary
                {
                    EventTime = DateTime.Now.AddDays(-1),
                    Action = "Reviewed",
                    ActionBy = "User3",
                    ActionByEmail = "user3@example.com",
                    Status = "Inactive",
                    CertificateData = JsonConvert.SerializeObject(new CertificateData
                    {
                        LearnerGivenNames = "Alice",
                        LearnerFamilyName = "Johnson",
                        EmployerName = "Company C",
                        StandardReference = "Ref3",
                        Version = "1.2",
                        IncidentNumber = "123"
                    }),
                BatchNumber = 3,
                ReasonForChange = "Reviewed status"
                }
            };
        }
    }
}
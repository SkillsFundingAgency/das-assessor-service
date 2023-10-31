using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    public class When_organisation_latest_withdrawal_date_is_requested
    {
        private Mock<IApplyRepository> _mockApplyRepository;
        private GetLatestWithdrawalDateForStandardHandler _sut;

        [SetUp]
        public void Setup()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ApplySummary, ApplicationResponse>();
            });

            _mockApplyRepository = new Mock<IApplyRepository>();
            
            _sut = new GetLatestWithdrawalDateForStandardHandler(_mockApplyRepository.Object);
        }

        [Test]
        public async Task Then_organisation_withdrawn_applications_are_retrieved()
        {
            // Arrange
            var expectedOrganisationId = Guid.NewGuid();
            var expectedStandardCode = 59;

            // Act
            await _sut.Handle(new GetLatestWithdrawalDateForStandardRequest(expectedOrganisationId, expectedStandardCode), new CancellationToken());
            
            // Assert
            _mockApplyRepository.Verify(r => r.GetLatestWithdrawalDateForStandard(expectedOrganisationId, expectedStandardCode), Times.Once);
        }
    }
}
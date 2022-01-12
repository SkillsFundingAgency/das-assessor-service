using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    public class When_organisation_all_withdrawn_applications_are_requested
    {

        private Mock<IApplyRepository> _mockApplyRepository;
        private GetAllWithdrawnApplicationsForStandardHandler _sut;

        [SetUp]
        public void Setup()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ApplySummary, ApplicationResponse>();
            });

            _mockApplyRepository = new Mock<IApplyRepository>();
            _mockApplyRepository.Setup(s => s.GetAllWithdrawnApplicationsForStandard(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(new List<ApplySummary> { new ApplySummary { StandardCode = 104, StandardApplicationType = "versionWithdrawal", ReviewStatus="Approved" } });

            _sut = new GetAllWithdrawnApplicationsForStandardHandler(_mockApplyRepository.Object);
        }


        [Test]
        public async Task Then_organisation_withdrawn_applications_are_retrieved()
        {
            await _sut.Handle(new GetAllWithdrawnApplicationsForStandardRequest(new Guid(), 59), new CancellationToken());
            _mockApplyRepository.Verify(r => r.GetAllWithdrawnApplicationsForStandard(It.IsAny<Guid>(),It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task Then_org_withdrawn_applications_are_retrieved_and_mapped_correctly()
        {
            //Arrange
            var request = new GetAllWithdrawnApplicationsForStandardRequest(new Guid("a623f742-2607-4bc0-85a7-ec5b6dd6a593"), 45);

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            result.Count.Should().Equals(1);
            result[0].StandardCode.Should().Equals(133);
        }

    }
}
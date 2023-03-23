using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    [TestFixture]
    public class When_organisation_previous_application_is_requested
    {

        private Mock<IApplyRepository> _mockApplyRepository;
        private GetPreviousApplicationsHandler _sut;

        [SetUp]
        public void Setup()
        {
            _mockApplyRepository = new Mock<IApplyRepository>();
            _mockApplyRepository.Setup(s => s.GetPreviousApplicationsForStandard(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ApplySummary> { new ApplySummary { StandardCode = 59 } });

            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ApplySummary, ApplicationResponse>();
            });

            _sut = new GetPreviousApplicationsHandler(_mockApplyRepository.Object);
        }


        [Test]
        public async Task Then_organisation_previous_application_is_retrieved()
        {
            await _sut.Handle(new GetPreviousApplicationsRequest(new Guid(), ""), new CancellationToken());
            _mockApplyRepository.Verify(r => r.GetPreviousApplicationsForStandard(It.IsAny<Guid>(),It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Then_org_previous_application_is_retrieved_and_mapped_correctly()
        {
            //Arrange
            var request = new GetPreviousApplicationsRequest(new Guid("a623f742-2607-4bc0-85a7-ec5b6dd6a593"), "ST0001");

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            result.Count.Should().Be(1);
            result[0].StandardCode.Should().Be(59);
        }

    }
}
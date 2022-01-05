using AutoMapper;
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
    public class When_organisation_previous_application_is_requested
    {

        private Mock<IApplyRepository> ApplyRepository;
        private GetPreviousApplicationsHandler Handler;

        [SetUp]
        public void Setup()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<List<ApplySummary>, List<ApplicationResponse>>();
            });

            ApplyRepository = new Mock<IApplyRepository>();
            Handler = new GetPreviousApplicationsHandler(ApplyRepository.Object);
        }


        [Test]
        public async Task Then_organisation_previous_application_is_retrieved()
        {
            await Handler.Handle(new GetPreviousApplicationsRequest(new Guid(), ""), new CancellationToken());
            ApplyRepository.Verify(r => r.GetPreviousApplication(It.IsAny<Guid>(),It.IsAny<string>()), Times.Once);
        }
    }
}
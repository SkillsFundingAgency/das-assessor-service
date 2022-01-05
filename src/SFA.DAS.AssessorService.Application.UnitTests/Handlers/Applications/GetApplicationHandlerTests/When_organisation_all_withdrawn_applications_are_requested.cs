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
    public class When_organisation_all_withdrawn_applications_are_requested
    {

        private Mock<IApplyRepository> ApplyRepository;
        private GetAllWithdrawnApplicationsForStandardHandler Handler;

        [SetUp]
        public void Setup()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<List<ApplySummary>, List<ApplicationResponse>>();
            });

            ApplyRepository = new Mock<IApplyRepository>();
            Handler = new GetAllWithdrawnApplicationsForStandardHandler(ApplyRepository.Object);
        }


        [Test]
        public async Task Then_organisation_withdrawn_applications_are_retrieved()
        {
            await Handler.Handle(new GetAllWithdrawnApplicationsForStandardRequest(new Guid(), 59), new CancellationToken());
            ApplyRepository.Verify(r => r.GetAllWithdrawnApplicationsForStandard(It.IsAny<Guid>(),It.IsAny<int>()), Times.Once);
        }
    }
}
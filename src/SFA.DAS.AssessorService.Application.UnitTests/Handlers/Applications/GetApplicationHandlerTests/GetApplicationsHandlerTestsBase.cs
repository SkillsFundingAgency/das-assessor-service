using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    [TestFixture]
    public class GetApplicationsHandlerTestsBase
    {
        protected Mock<IApplyRepository> ApplyRepository;
        protected GetApplicationsHandler Handler;   

        [SetUp]
        public void Setup()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<List<Domain.Entities.Apply>, List<ApplicationResponse>>();
            });

            ApplyRepository = new Mock<IApplyRepository>();
            ApplyRepository.Setup(r => r.GetUserApplications(It.IsAny<Guid>())).ReturnsAsync(new List<Domain.Entities.Apply>());
            ApplyRepository.Setup(r => r.GetOrganisationApplications(It.IsAny<Guid>())).ReturnsAsync(new List<Domain.Entities.Apply>());
            Handler = new GetApplicationsHandler(ApplyRepository.Object);
        }
    }
}
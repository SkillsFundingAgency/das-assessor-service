using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.WithdrawalApplicationsHandlerTests
{
    [TestFixture]
    public class WithdrawalApplicationHandlerTestsBase
    {
        protected Mock<IApplyRepository> ApplyRepository;
        protected Mock<ILogger<WithdrawalApplicationsHandler>> Logger;
        protected Mock<IStandardRepository> StandardRepository;
        protected WithdrawalApplicationsHandler Handler;

        [SetUp]
        public void Setup()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ApplicationListItem, ApplicationSummaryItem>()
                .ForMember(dest => dest.Versions, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<string>>(src.Versions)))
                ;
            });

            ApplyRepository = new Mock<IApplyRepository>();
                    
            Logger = new Mock<ILogger<WithdrawalApplicationsHandler>>();
            
            StandardRepository = new Mock<IStandardRepository>();
            StandardRepository.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<OrganisationStandardVersion>() {
                        new OrganisationStandardVersion() { Version = "1.0m" },
                        new OrganisationStandardVersion() { Version = "1.1m" },
                        new OrganisationStandardVersion() { Version = "1.2m" },
                        new OrganisationStandardVersion() { Version = "1.3m" },
                    });

            Handler = new WithdrawalApplicationsHandler(ApplyRepository.Object, Logger.Object, StandardRepository.Object);
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FizzWare.NBuilder;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class OrganisationQueryBase
    {
        protected OrganisationQueryController OrganisationQueryController;
        protected UkPrnValidator UkPrnValidator;

        protected Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;

        protected Mock<IMediator> Mediator = new Mock<IMediator>();

        protected Mock<IStringLocalizer<OrganisationQueryController>> OrganisationControllerLocaliserMock;

        protected Mock<ILogger<OrganisationQueryController>> ControllerLoggerMock;

        protected Mock<IWebConfiguration> ConfigMock = new Mock<IWebConfiguration>();


        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;

        protected  void Setup()
        {
            Mediator = new Mock<IMediator>();

            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Organisation, OrganisationResponse>();
            });
            EpaoStandardsCountResponse response = new EpaoStandardsCountResponse(1);
         
            Mediator.Setup(q => q.Send(Moq.It.IsAny<GetEpaoStandardsCountRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(response));
            SetupControllerMocks();

            OrganisationQueryController = new OrganisationQueryController(ControllerLoggerMock.Object, OrganisationQueryRepositoryMock.Object, UkPrnValidator, OrganisationControllerLocaliserMock.Object, ConfigMock.Object);
        }

        private void SetupControllerMocks()
        {
            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            OrganisationControllerLocaliserMock = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<OrganisationQueryController>();

            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            var ukPrnStringLocalizer = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.InvalidUkprn)
                .WithKeyValue("100000000")
                .Build<UkPrnValidator>();

            UkPrnValidator = new UkPrnValidator(ukPrnStringLocalizer.Object);

            ControllerLoggerMock = new Mock<ILogger<OrganisationQueryController>>();
        }
        
    }
}

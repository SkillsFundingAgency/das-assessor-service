using System;
using System.Threading;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    public class RegisterUpdateTestsBase
    {
        protected Mock<IAssessmentOrgsApiClient> ApiClient;
        protected Mock<IOrganisationQueryRepository> OrganisationRepository;
        protected RegisterUpdateHandler RegisterUpdateHandler;
        protected Mock<ILogger<RegisterUpdateHandler>> Logger;
        protected Mock<IMediator> Mediator;

        protected void Setup()
        {
            ApiClient = new Mock<IAssessmentOrgsApiClient>();

            OrganisationRepository = new Mock<IOrganisationQueryRepository>();

            Logger = new Mock<ILogger<RegisterUpdateHandler>>();

            Mediator = new Mock<IMediator>();

            Mediator.Setup(m => m.Send(It.IsAny<OrganisationCreateViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OrganisationQueryViewModel() {Id = new Guid()});

            RegisterUpdateHandler = new RegisterUpdateHandler(ApiClient.Object, OrganisationRepository.Object, Logger.Object, Mediator.Object);
        }
    }
}
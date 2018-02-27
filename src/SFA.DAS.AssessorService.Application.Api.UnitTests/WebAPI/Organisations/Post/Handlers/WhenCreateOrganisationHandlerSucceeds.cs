namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Post.Handlers
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Moq;
    using SFA.DAS.AssessorService.Application.OrganisationHandlers;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading;
    using System.Threading.Tasks;
    using SFA.DAS.AssessorService.Api.Types;

    [Subject("AssessorService")]
    public class WhenCreateOrganisationHandlerSucceeds
    {
        private static CreateOrganisationHandler CreateOrganisationHandler;
        protected static Mock<IOrganisationRepository> OrganisationRepositoryMock;
        protected static OrganisationCreateDomainModel _organisationCreateDomainModel;
        protected static Organisation _organisationQueryViewModel;
        protected static CreateOrganisationRequest _organisationCreateViewModel;
        protected static Organisation _result;

        Establish context = () =>
        {
            Bootstrapper.Initialize();

            OrganisationRepositoryMock = new Mock<IOrganisationRepository>();

            _organisationCreateDomainModel = Builder<OrganisationCreateDomainModel>.CreateNew().Build();
            _organisationQueryViewModel = Builder<Organisation>.CreateNew().Build();
            _organisationCreateViewModel = Builder<CreateOrganisationRequest>.CreateNew().Build();

            OrganisationRepositoryMock.Setup(q => q.CreateNewOrganisation(Moq.It.IsAny<OrganisationCreateDomainModel>()))
                        .Returns(Task.FromResult((_organisationQueryViewModel)));

            CreateOrganisationHandler = new CreateOrganisationHandler(OrganisationRepositoryMock.Object);
        };

        Because of = () =>
        {
            _result = CreateOrganisationHandler.Handle(_organisationCreateViewModel, new CancellationToken()).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = _result as Organisation;
            result.Should().NotBeNull();
        };
    }
}


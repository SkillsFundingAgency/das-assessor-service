namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Post.Handlers
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Moq;
    using SFA.DAS.AssessorService.Application.CreateOrganisation;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenCreateOrganisationHandlerSucceeds
    {
        private static CreateOrganisationHandler CreateOrganisationHandler;
        protected static Mock<IOrganisationRepository> OrganisationRepositoryMock;
        protected static OrganisationCreateDomainModel _organisationCreateDomainModel;
        protected static OrganisationQueryViewModel _organisationQueryViewModel;
        protected static OrganisationCreateViewModel _organisationCreateViewModel;
        protected static OrganisationQueryViewModel _result;

        Establish context = () =>
        {
            Bootstrapper.Initialize();

            OrganisationRepositoryMock = new Mock<IOrganisationRepository>();

            _organisationCreateDomainModel = Builder<OrganisationCreateDomainModel>.CreateNew().Build();
            _organisationQueryViewModel = Builder<OrganisationQueryViewModel>.CreateNew().Build();
            _organisationCreateViewModel = Builder<OrganisationCreateViewModel>.CreateNew().Build();

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
            var result = _result as OrganisationQueryViewModel;
            result.Should().NotBeNull();
        };
    }
}


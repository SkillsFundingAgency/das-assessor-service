namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Post.Handlers
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using SFA.DAS.AssessorService.Application.OrganisationHandlers;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Data;
    using SFA.DAS.AssessorService.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Organisation = AssessorService.Api.Types.Models.Organisation;

    [Subject("AssessorService")]
    public class WhenCreateOrganisationPersistsData
    {
        private static OrganisationRepository _organisationRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;
        private static OrganisationCreateDomainModel _organisationCreateDomainModel;
        private static Mock<DbSet<AssessorService.Domain.Entities.Organisation>> _organisationDBSetMock;
        //private static CreateOrganisationHandler CreateOrganisationHandler;
        //protected static Mock<IOrganisationRepository> OrganisationRepositoryMock;
        //protected static OrganisationCreateDomainModel _organisationCreateDomainModel;
        protected static Organisation _result;
        //protected static OrganisationCreateViewModel _organisationCreateViewModel;
        //protected static OrganisationQueryViewModel _result;

        Establish context = () =>
        {
            MappingBootstrapper.Initialize();

            _organisationCreateDomainModel = Builder<OrganisationCreateDomainModel>.CreateNew().Build();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _organisationDBSetMock = new Mock<DbSet<AssessorService.Domain.Entities.Organisation>>();

            var mockSet = new Mock<DbSet<AssessorService.Domain.Entities.Organisation>>();
            var mockContext = new Mock<AssessorDbContext>();

            var organisations = new List<AssessorService.Domain.Entities.Organisation>();

            mockSet.Setup(m => m.Add(Moq.It.IsAny<AssessorService.Domain.Entities.Organisation>())).Callback((AssessorService.Domain.Entities.Organisation organisation) => organisations.Add(organisation));

            _assessorDbContext.Setup(q => q.Organisations).Returns(mockSet.Object);
            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

            _organisationRepository = new OrganisationRepository(_assessorDbContext.Object);

        };

        Because of = () =>
        {
            _result = _organisationRepository.CreateNewOrganisation(_organisationCreateDomainModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            _result.Should().NotBeNull();
        };
    }
}


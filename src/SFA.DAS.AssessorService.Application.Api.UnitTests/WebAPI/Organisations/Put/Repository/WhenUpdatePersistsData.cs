namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Put.Repository
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using SFA.DAS.AssessorService.Data;
    using SFA.DAS.AssessorService.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using Domain;

    [Subject("AssessorService")]
    public class WhenUpdatePersistsData
    {
        private static OrganisationRepository _organisationRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;
        private static OrganisationUpdateDomainModel _organisationUpdateDomainModel;
        private static Mock<DbSet<Organisation>> _organisationDBSetMock;

        protected static AssessorService.Api.Types.Models.Organisation _result;


        Establish context = () =>
        {
            MappingBootstrapper.Initialize();

            _organisationUpdateDomainModel = Builder<OrganisationUpdateDomainModel>.CreateNew().Build();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _organisationDBSetMock = new Mock<DbSet<AssessorService.Domain.Entities.Organisation>>();

            var mockSet = new Mock<DbSet<AssessorService.Domain.Entities.Organisation>>();
            var mockContext = new Mock<AssessorDbContext>();


            var organisations = new List<AssessorService.Domain.Entities.Organisation>
            {
                Builder<AssessorService.Domain.Entities.Organisation>.CreateNew().Build()
            }.AsQueryable();

            mockSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.Provider).Returns(organisations.Provider);
            mockSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.Expression).Returns(organisations.Expression);
            mockSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.ElementType).Returns(organisations.ElementType);
            mockSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.GetEnumerator()).Returns(organisations.GetEnumerator());

            mockContext.Setup(c => c.Organisations).Returns(mockSet.Object);

            _assessorDbContext.Setup(q => q.Organisations).Returns(mockSet.Object);
            _assessorDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<AssessorService.Domain.Entities.Organisation>()));


            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

            _organisationRepository = new OrganisationRepository(_assessorDbContext.Object);

        };

        Because of = () =>
        {
            _result = _organisationRepository.UpdateOrganisation(_organisationUpdateDomainModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = (_result as AssessorService.Api.Types.Models.Organisation);
            result.Should().NotBeNull();
        };
    }
}


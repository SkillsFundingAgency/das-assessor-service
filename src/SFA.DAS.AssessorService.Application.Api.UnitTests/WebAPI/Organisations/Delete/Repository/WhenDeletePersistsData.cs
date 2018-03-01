namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Put.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Domain.Entities;
    using Data;
    using Domain;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using It = Machine.Specifications.It;

    [Subject("AssessorService")]
    public class WhenDeletePersistsData
    {
        private static OrganisationRepository _organisationRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;
        private static OrganisationUpdateDomainModel _organisationUpdateDomainModel;
        private static Mock<DbSet<Organisation>> _organisationDBSetMock;

        protected static Task _result;

        Establish context = () =>
        {
            MappingBootstrapper.Initialize();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _organisationDBSetMock = new Mock<DbSet<Organisation>>();

            var mockSet = new Mock<DbSet<Organisation>>();
            var mockContext = new Mock<AssessorDbContext>();


            var organisations = new List<Organisation>
            {
                Builder<Organisation>.CreateNew()
                .With(q => q.EndPointAssessorUKPRN = 10000000)
                .Build()
            }.AsQueryable();

            mockSet.As<IQueryable<Organisation>>().Setup(m => m.Provider).Returns(organisations.Provider);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.Expression).Returns(organisations.Expression);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.ElementType).Returns(organisations.ElementType);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.GetEnumerator()).Returns(organisations.GetEnumerator());

            mockContext.Setup(c => c.Organisations).Returns(mockSet.Object);

            _assessorDbContext.Setup(q => q.Organisations).Returns(mockSet.Object);
            _assessorDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Organisation>()));


            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

            _organisationRepository = new OrganisationRepository(_assessorDbContext.Object);

        };

        Because of = () =>
        {
            _result = _organisationRepository.Delete("123456");
        };

        It verify_succesfully = () =>
        {
            var taskresult = _result;
            taskresult.Should().NotBeNull();
        };
    }
}



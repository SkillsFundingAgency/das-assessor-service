using SFA.DAS.AssessorService.Domain.DomainModels;

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
    using System;

    [Subject("AssessorService")]
    public class WhenUpdatePersistsData
    {
        private static OrganisationRepository _organisationRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;
        private static OrganisationUpdateDomainModel _organisationUpdateDomainModel;
        private static Mock<DbSet<Organisation>> _organisationDBSetMock;

        protected static AssessorService.Api.Types.Models.Organisation _result;


        private Establish context = () =>
        {
            MappingBootstrapper.Initialize();

            _organisationUpdateDomainModel = Builder<OrganisationUpdateDomainModel>
                .CreateNew()
                .With(q => q.PrimaryContact = "TestUser")
                .Build();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _organisationDBSetMock = new Mock<DbSet<AssessorService.Domain.Entities.Organisation>>();

            var organisationMockDbSet = new Mock<DbSet<AssessorService.Domain.Entities.Organisation>>();
            var contactsMockDbSet = new Mock<DbSet<AssessorService.Domain.Entities.Contact>>();

            var mockContext = new Mock<AssessorDbContext>();

            var primaryContactId = Guid.NewGuid();

            var organisations = new List<AssessorService.Domain.Entities.Organisation>
            {
                Builder<AssessorService.Domain.Entities.Organisation>.CreateNew()
                    .With(q => q.PrimaryContactId = primaryContactId)
                    .Build()
            }.AsQueryable();

            var contacts = new List<AssessorService.Domain.Entities.Contact>
            {
                Builder<AssessorService.Domain.Entities.Contact>.CreateNew()
                    .With(q => q.Id = primaryContactId)
                    .With(q => q.Username="TestUser")
                    .Build()
            }.AsQueryable();

            organisationMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.Provider).Returns(organisations.Provider);
            organisationMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.Expression).Returns(organisations.Expression);
            organisationMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.ElementType).Returns(organisations.ElementType);
            organisationMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Organisation>>().Setup(m => m.GetEnumerator()).Returns(organisations.GetEnumerator());

            contactsMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Contact>>().Setup(m => m.Provider).Returns(contacts.Provider);
            contactsMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Contact>>().Setup(m => m.Expression).Returns(contacts.Expression);
            contactsMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Contact>>().Setup(m => m.ElementType).Returns(contacts.ElementType);
            contactsMockDbSet.As<IQueryable<AssessorService.Domain.Entities.Contact>>().Setup(m => m.GetEnumerator()).Returns(contacts.GetEnumerator());

            mockContext.Setup(c => c.Organisations).Returns(organisationMockDbSet.Object);
            mockContext.Setup(c => c.Contacts).Returns(contactsMockDbSet.Object);

            _assessorDbContext.Setup(q => q.Organisations).Returns(organisationMockDbSet.Object);
            _assessorDbContext.Setup(q => q.Contacts).Returns(contactsMockDbSet.Object);
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


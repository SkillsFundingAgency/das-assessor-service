using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Organisations
{
    public class WhenUpdateOrganisationPersistsData
    {
        private static Organisation _result;
        private string _primaryContact = "TestUser";

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var organisationUpdateDomainModel = Builder<Organisation>
                .CreateNew()
                .With(q => q.PrimaryContact = _primaryContact)
                .Build();


            var primaryContactId = Guid.NewGuid();
            var organisationMockDbSet = CreateOrganisationMockDbSet(primaryContactId);
            var contactsMockDbSet = CreateContactsMockDbSet(primaryContactId);

            var mockDbContext = CreateMockDbContext(organisationMockDbSet, contactsMockDbSet);

            var organisationRepository = new OrganisationRepository(mockDbContext.Object);
            _result = organisationRepository.UpdateOrganisation(organisationUpdateDomainModel).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            var result = (_result as Organisation);
            result.Should().NotBeNull();
        }

        private Mock<DbSet<Contact>> CreateContactsMockDbSet(Guid primaryContactId)
        {
            var contacts = new List<Contact>
            {
                Builder<Contact>.CreateNew()
                    .With(q => q.Id = primaryContactId)
                    .With(q => q.Username = _primaryContact)
                    .Build()
            }.AsQueryable();

            var contactsMockDbSet = contacts.CreateMockSet(contacts);
            return contactsMockDbSet;
        }

        private Mock<DbSet<Organisation>> CreateOrganisationMockDbSet(Guid primaryContactId)
        {
            var organisations = new List<Organisation>
            {
                Builder<Organisation>.CreateNew()
                    .With(q => q.PrimaryContact = _primaryContact)
                    .Build()
            }.AsQueryable();

            var organisationMockDbSet = organisations.CreateMockSet(organisations);
            return organisationMockDbSet;
        }

        private Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Organisation>> organisationMockDbSet, Mock<DbSet<Contact>> contactsMockDbSet)
        {
            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.Organisations).Returns(organisationMockDbSet.Object);
            mockDbContext.Setup(c => c.Contacts).Returns(contactsMockDbSet.Object);

            mockDbContext.Setup(q => q.Organisations).Returns(organisationMockDbSet.Object);
            mockDbContext.Setup(q => q.Contacts).Returns(contactsMockDbSet.Object);
            mockDbContext.Setup(x => x.MarkAsModified(It.IsAny<Organisation>()));

            mockDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((It.IsAny<int>())));
            return mockDbContext;
        }
    }
}


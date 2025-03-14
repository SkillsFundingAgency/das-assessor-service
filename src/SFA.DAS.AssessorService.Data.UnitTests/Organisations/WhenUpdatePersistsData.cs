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
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Organisations
{
    public class WhenUpdateOrganisationPersistsData
    {
        private static Organisation _result;
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork;
        
        private readonly string _primaryContact = "TestUser";

        [SetUp]
        public void Arrange()
        {
            var organisationUpdateDomainModel = Builder<Organisation>
                .CreateNew()
                .With(q => q.PrimaryContact = _primaryContact)
                .Build();

            var primaryContactId = Guid.NewGuid();
            var contactsMockDbSet = CreateContactsMockDbSet(primaryContactId);
            var organisationMockDbSet = CreateOrganisationMockDbSet();

            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork.Setup(p => p.AssessorDbContext).Returns(CreateMockDbContext(organisationMockDbSet, contactsMockDbSet).Object);

            var organisationRepository = new OrganisationRepository(_mockAssessorUnitOfWork.Object);
            _result = organisationRepository.UpdateOrganisation(organisationUpdateDomainModel).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Should().NotBeNull();
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

            var contactsMockDbSet = contacts.CreateMockSet();
            return contactsMockDbSet;
        }

        private Mock<DbSet<Organisation>> CreateOrganisationMockDbSet()
        {
            var organisations = new List<Organisation>
            {
                Builder<Organisation>.CreateNew()
                    .With(q => q.PrimaryContact = _primaryContact)
                    .Build()
            }.AsQueryable();

            var organisationMockDbSet = organisations.CreateMockSet();
            return organisationMockDbSet;
        }

        private Mock<IAssessorDbContext> CreateMockDbContext(Mock<DbSet<Organisation>> organisationMockDbSet, Mock<DbSet<Contact>> contactsMockDbSet)
        {
            var mockDbContext = new Mock<IAssessorDbContext>();
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


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Contacts
{
    public class WhenCreateContactPersistsData
    {
        private ContactRepository _contactRepository;
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork;
        private Contact _result;

        [SetUp]
        public void Arrange()
        {
            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork.Setup(p => p.AssessorDbContext).Returns(CreateMockDbContext(CreateMockSet()).Object);

            _contactRepository = new ContactRepository(_mockAssessorUnitOfWork.Object, new Mock<IUnitOfWork>().Object);
            
            var contactCreateDomainModel = Builder<Contact>.CreateNew().Build();
            _result = _contactRepository.CreateNewContact(contactCreateDomainModel).Result;
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _result.Should().NotBeNull();
        }

        private Mock<IAssessorDbContext> CreateMockDbContext(Mock<DbSet<Contact>> mockSet)
        {
            var assessorDbContext = new Mock<IAssessorDbContext>();
            assessorDbContext.Setup(q => q.Contacts).Returns(mockSet.Object);
            assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((It.IsAny<int>())));
            
            return assessorDbContext;
        }

        private Mock<DbSet<Contact>> CreateMockSet()
        {
            var mockSet = new Mock<DbSet<Contact>>();
            var organisations = new List<Contact>();
            mockSet.Setup(m => m.Add(It.IsAny<Contact>()))
                .Callback((Contact organisation) => organisations.Add(organisation));
            return mockSet;
        }
    }
}


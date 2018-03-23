using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Data.UnitTests.Contacts
{
    public class WhenCreateContactPersistsData
    {
        private ContactRepository _contactRepository;
        private Mock<AssessorDbContext> _assessorDbContext;
        private ContactResponse _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var contactCreateDomainModel = Builder<CreateContactDomainModel>.CreateNew().Build();

            var mockSet = CreateMockSet();
            CreateMockDbCOntext(mockSet);

            _contactRepository = new ContactRepository(_assessorDbContext.Object);
            _result = _contactRepository.CreateNewContact(contactCreateDomainModel).Result;

        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _result.Should().NotBeNull();
        }

        private void CreateMockDbCOntext(Mock<DbSet<Domain.Entities.Contact>> mockSet)
        {
            _assessorDbContext = new Mock<AssessorDbContext>();
            _assessorDbContext.Setup(q => q.Contacts).Returns(mockSet.Object);
            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));
        }

        private Mock<DbSet<Domain.Entities.Contact>> CreateMockSet()
        {
            var mockSet = new Mock<DbSet<AssessorService.Domain.Entities.Contact>>();
            var organisations = new List<AssessorService.Domain.Entities.Contact>();
            mockSet.Setup(m => m.Add(Moq.It.IsAny<AssessorService.Domain.Entities.Contact>()))
                .Callback((AssessorService.Domain.Entities.Contact organisation) => organisations.Add(organisation));
            return mockSet;
        }
    }
}


﻿using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.UnitTests.Contacts
{
    public class WhenCreateContactPersistsData
    {
        private ContactRepository _contactRepository;
        private Mock<AssessorDbContext> _assessorDbContext;
        private Contact _result;

        [SetUp]
        public void Arrange()
        {
            var contactCreateDomainModel = Builder<Contact>.CreateNew().Build();

            var mockSet = CreateMockSet();
            CreateMockDbContext(mockSet);

            _contactRepository = new ContactRepository(_assessorDbContext.Object, new Mock<IUnitOfWork>().Object);
            _result = _contactRepository.CreateNewContact(contactCreateDomainModel).Result;

        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _result.Should().NotBeNull();
        }

        private void CreateMockDbContext(Mock<DbSet<Contact>> mockSet)
        {
            _assessorDbContext = new Mock<AssessorDbContext>();
            _assessorDbContext.Setup(q => q.Contacts).Returns(mockSet.Object);
            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((It.IsAny<int>())));
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


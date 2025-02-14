﻿using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.UnitTests.Contacts
{
    public class WhenDeleteContactPersistsData
    {
        private Mock<AssessorDbContext> _mockDbContext = new Mock<AssessorDbContext>();

        [SetUp]
        public async Task Arrange()
        {
            _mockDbContext = new Mock<AssessorDbContext>();
                 
           var contacts = new List<Contact>
            {
                Builder<Contact>.CreateNew()    
                  .With(q => q.Username = "1234")
                    .Build()
            }.AsQueryable();

            var mockSet = contacts.CreateMockSet(contacts);
            
            _mockDbContext.Setup(q => q.Contacts).Returns(mockSet.Object);
            _mockDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Contact>()));
            _mockDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

            var contactRepository = new ContactRepository(_mockDbContext.Object, new Mock<IUnitOfWork>().Object);

            await contactRepository.Delete("1234");
        }

        [Test]
        public void ThenDeleteOrganisationShouldSucceed()
        {
            _mockDbContext.Verify(q => q.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}



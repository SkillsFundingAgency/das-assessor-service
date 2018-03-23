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
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data.UnitTests.Contacts
{  
    public class WhenDeletePersistsDataAndIsNotFound
    {
        private Exception _exception;

        [SetUp]
        public async Task Arrange()
        {
            MappingBootstrapper.Initialize();

            var userNameToFind = "NotFoundUser";
           
            var contacts = new List<Contact>
            {
                Builder<Contact>.CreateNew()
                    .With(q => q.Username = userNameToFind)
                    .Build()
            }.AsQueryable();

            var mockSet = contacts.CreateMockSet(contacts);
            var mockDbContext = CreateMockDbContext(mockSet);

            var contactRepository = new ContactRepository(mockDbContext.Object);

            try
            {
               await contactRepository.Delete("testuser");
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
        
        }                      

        [Test]
        public void ThenNotFoundExceptionSHouldBeThrown()
        {
            _exception.Should().BeOfType<NotFound>();            
        }

        private Mock<AssessorDbContext> CreateMockDbContext(IMock<DbSet<Contact>> mockSet)
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            mockDbContext.Setup(c => c.Contacts).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Contact>()));

            mockDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult(Moq.It.IsAny<int>()));
            return mockDbContext;
        }
    }
}
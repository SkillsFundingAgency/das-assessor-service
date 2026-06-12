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
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data.UnitTests.Contacts
{
    public class WhenDeletePersistsDataAndIsNotFound
    {
        private Exception _exception;
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork;

        [SetUp]
        public async Task Arrange()
        {
            var userNameToFind = "NotFoundUser";
           
            var contacts = new List<Contact>
            {
                Builder<Contact>.CreateNew()
                    .With(q => q.Username = userNameToFind)
                    .Build()
            }.AsQueryable();

            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork.Setup(p => p.AssessorDbContext).Returns(CreateMockDbContext(contacts.CreateMockSet()).Object);

            var contactRepository = new ContactRepository(_mockAssessorUnitOfWork.Object, new Mock<IUnitOfWork>().Object);

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
            _exception.Should().BeOfType<NotFoundException>();            
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
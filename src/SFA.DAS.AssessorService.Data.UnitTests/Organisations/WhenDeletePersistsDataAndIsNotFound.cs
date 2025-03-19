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

namespace SFA.DAS.AssessorService.Data.UnitTests.Organisations
{  
    public class WhenDeletePersistsDataAndIsNotFound
    {
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork;
        private OrganisationRepository _organisationRepository;
        private Exception _exception;

        [SetUp]
        public async Task Arrange()
        {
            var organisations = new List<Organisation>
            {
                Builder<Organisation>.CreateNew()
                    .With(q => q.EndPointAssessorUkprn = 10000000)
                    .Build()
            }.AsQueryable();

            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork.Setup(p => p.AssessorDbContext).Returns(CreateMockDbContext(organisations.CreateMockSet()).Object);

            _organisationRepository = new OrganisationRepository(_mockAssessorUnitOfWork.Object);

            try
            {
               await _organisationRepository.Delete("123456");
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

        private Mock<IAssessorDbContext> CreateMockDbContext(IMock<DbSet<Organisation>> mockSet)
        {
            var mockDbContext = new Mock<IAssessorDbContext>();

            mockDbContext.Setup(c => c.Organisations).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.MarkAsModified(It.IsAny<Organisation>()));

            mockDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult(It.IsAny<int>()));
            
            return mockDbContext;
        }
    }
}
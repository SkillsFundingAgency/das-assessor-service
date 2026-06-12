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

namespace SFA.DAS.AssessorService.Data.UnitTests.Organisations
{
    public class WhenCreateOrganisationPersistsData
    {
        private OrganisationRepository _organisationRepository;
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork;
        private Organisation _result;
        
        [SetUp]
        public void Arrange()
        { 
            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork.Setup(p => p.AssessorDbContext).Returns(CreateMockDbContext(CreateMockDbSet()).Object);

            _organisationRepository = new OrganisationRepository(_mockAssessorUnitOfWork.Object);
            
            var organisation = Builder<Organisation>.CreateNew().Build();
            _result = _organisationRepository.CreateNewOrganisation(organisation).Result;
        }     

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Should().NotBeNull();
        }

        [Test]
        public void ItShouldReturnAnOrganisation()
        {
            _result.Should().BeOfType<Organisation>();
        }

        [Test]
        public void ItShouldPersistTheData()
        {
            _mockAssessorUnitOfWork.Verify(q => q.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Organisation>> mockSet)
        {
            var mockContext = new Mock<AssessorDbContext>();
            mockContext.Setup(q => q.Organisations).Returns(mockSet.Object);
            mockContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((It.IsAny<int>())));
            return mockContext;
        }

        private static Mock<DbSet<Organisation>> CreateMockDbSet()
        {
            var mockSet = new Mock<DbSet<Organisation>>();
            var organisations = new List<Organisation>();
            mockSet.Setup(m => m.Add(It.IsAny<Organisation>()))
                .Callback((Organisation organisation) => organisations.Add(organisation));
            return mockSet;
        }
    }
}


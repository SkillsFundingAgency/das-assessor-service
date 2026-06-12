using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Organisations
{
    public class WhenDeletePersistsSuccceds
    {
        private OrganisationRepository _organisationRepository;
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork;

        [SetUp]
        public async Task Arrange()
        {
            var organisations = new List<Organisation>
            {
                Builder<Organisation>.CreateNew()
                    .With(q => q.EndPointAssessorOrganisationId = "123456")
                    .With(q => q.Status = OrganisationStatus.Live)
                    .With(q => q.EndPointAssessorUkprn = 10000000)
                    .Build()
            }.AsQueryable();

            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork.Setup(p => p.AssessorDbContext).Returns(CreateMockDbContext(organisations.CreateMockSet()).Object);

            _organisationRepository = new OrganisationRepository(_mockAssessorUnitOfWork.Object);
           
            await _organisationRepository.Delete("123456");
        }

        [Test]
        public void ThenDeleteOrganisationShouldSucceed()
        {
            _mockAssessorUnitOfWork.Verify(q => q.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private Mock<IAssessorDbContext> CreateMockDbContext(Mock<DbSet<Organisation>> mockSet)
        {
            var mockDbContext = new Mock<IAssessorDbContext>();

            mockDbContext.Setup(c => c.Organisations).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Organisation>()));

            mockDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult(Moq.It.IsAny<int>()));
            return mockDbContext;
        }
    }
}
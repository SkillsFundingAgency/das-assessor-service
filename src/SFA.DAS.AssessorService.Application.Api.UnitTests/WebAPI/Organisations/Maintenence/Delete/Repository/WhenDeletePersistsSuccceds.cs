using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Delete.Repository
{
    public class WhenDeletePersistsSuccceds
    {
        private OrganisationRepository _organisationRepository;    
        private Mock<AssessorDbContext> _mockDbContext = new Mock<AssessorDbContext>();

        [SetUp]
        public async Task Arrange()
        {
            MappingBootstrapper.Initialize();

            var organisations = new List<Organisation>
            {
                Builder<Organisation>.CreateNew()
                    .With(q => q.EndPointAssessorOrganisationId = "123456")
                    .With(q => q.Status = OrganisationStatus.Live)
                    .With(q => q.EndPointAssessorUkprn = 10000000)
                    .Build()
            }.AsQueryable();

            var mockSet = organisations.CreateMockSet(organisations);
            _mockDbContext = CreateMockDbContext(mockSet);
                                
            _organisationRepository = new OrganisationRepository(_mockDbContext.Object);
           
            await _organisationRepository.Delete("123456");
        }

        [Test]
        public void ThenDeleteOrganisationShouldSucceed()
        {
            _mockDbContext.Verify(q => q.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Organisation>> mockSet)
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            mockDbContext.Setup(c => c.Organisations).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Organisation>()));

            mockDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult(Moq.It.IsAny<int>()));
            return mockDbContext;
        }
    }
}
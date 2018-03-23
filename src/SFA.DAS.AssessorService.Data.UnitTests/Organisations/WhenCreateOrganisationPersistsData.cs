using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Data.UnitTests.Organisations
{
    using OrganisationResponse = Api.Types.Models.OrganisationResponse;
   
    public class WhenCreateOrganisationPersistsData
    {
        private  OrganisationRepository _organisationRepository;
        private Mock<AssessorDbContext> _mockDbContext;
        private  OrganisationResponse _result;
        
        [SetUp]
        public void Arrange()
        { 
            MappingBootstrapper.Initialize();

            var organisationCreateDomainModel = Builder<CreateOrganisationDomainModel>.CreateNew().Build();           
                       
            var mockSet = CreateMockDbSet();
            _mockDbContext = CreateMockDbContext(mockSet);

            _organisationRepository = new OrganisationRepository(_mockDbContext.Object);
            _result = _organisationRepository.CreateNewOrganisation(organisationCreateDomainModel).Result;
        }     

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Should().NotBeNull();
        }

        [Test]
        public void ItShouldReturnAnOrganisation()
        {
            _result.Should().BeOfType<OrganisationResponse>();
        }

        [Test]
        public void ItShouldPersistTheData()
        {
            _mockDbContext.Verify(q => q.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Domain.Entities.Organisation>> mockSet)
        {
            var mockContext = new Mock<AssessorDbContext>();
            mockContext.Setup(q => q.Organisations).Returns(mockSet.Object);
            mockContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));
            return mockContext;
        }

        private static Mock<DbSet<Domain.Entities.Organisation>> CreateMockDbSet()
        {
            var mockSet = new Mock<DbSet<Domain.Entities.Organisation>>();
            var organisations = new List<Domain.Entities.Organisation>();
            mockSet.Setup(m => m.Add(Moq.It.IsAny<Domain.Entities.Organisation>()))
                .Callback((Domain.Entities.Organisation organisation) => organisations.Add(organisation));
            return mockSet;
        }
    }
}


using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Organisations
{
 public class WhenCreateOrganisationHandlerSucceeds
    {
        private Mock<IOrganisationRepository> _organisationRepositoryMock;
        private Mock<IContactRepository> _contactRepositoryMock;
        private Organisation _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            CreateContractRepositoryMock();
            CreateOrganisationRepositoryMock();

            var createOrganisationRequest = Builder<CreateOrganisationRequest>.CreateNew().Build();        
            var organisationQueryRepository = CreateOrganisationQueryRepository();

            var createOrganisationHandler = new CreateOrganisationHandler(_organisationRepositoryMock.Object, 
                organisationQueryRepository.Object,
                _contactRepositoryMock.Object);
            _result = createOrganisationHandler.Handle(createOrganisationRequest, new CancellationToken()).Result;
        }

        private void CreateContractRepositoryMock()
        {
            _contactRepositoryMock = new Mock<IContactRepository>();
        }

        [Test]
        public void ItShouldReturnAResult()
        { 
            var result = _result as Organisation;
            result.Should().NotBeNull();
        }


        [Test]
        public void ItShouldReturnAnOrganisation()
        {
            _result.Should().BeOfType<Organisation>();
        }

        [Test]
        public void ItShouldPersistTheData()
        {
            _organisationRepositoryMock.Verify(q => q.CreateNewOrganisation(It.IsAny<Organisation>()), Times.Once);
        }

        private static Mock<IOrganisationQueryRepository> CreateOrganisationQueryRepository()
        {
            var organisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            organisationQueryRepository.Setup(r => r.GetByUkPrn(It.IsAny<int>())).ReturnsAsync(default(Organisation));
            return organisationQueryRepository;
        }

        private void CreateOrganisationRepositoryMock()
        {
            _organisationRepositoryMock = new Mock<IOrganisationRepository>();
            var organisation = Builder<Organisation>.CreateNew().Build();
            _organisationRepositoryMock.Setup(q => q.CreateNewOrganisation(It.IsAny<Organisation>()))
                .Returns(Task.FromResult((organisation)));
        }
    }
}


using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DomainModels;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Organisations
{
    [TestFixture]
    public class WhenDeletedOrganisationExists
    {
        private CreateOrganisationHandler _handler;
        private Mock<IOrganisationRepository> _orgRepos;
        private Mock<IContactRepository> _contactRepository;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            _orgRepos = new Mock<IOrganisationRepository>();
            _contactRepository = new Mock<IContactRepository>();

            var orgQueryRepos = new Mock<IOrganisationQueryRepository>();
            orgQueryRepos.Setup(r => r.GetByUkPrn(It.IsAny<int>())).ReturnsAsync(new Organisation()
            {
                Status = OrganisationStatus.Deleted,
                EndPointAssessorOrganisationId = "12345"
            });

            _orgRepos.Setup(r => r.UpdateOrganisation(It.IsAny<UpdateOrganisationDomainModel>()))
                .ReturnsAsync(new OrganisationResponse());

            _handler = new CreateOrganisationHandler(_orgRepos.Object,             
                orgQueryRepos.Object,
                _contactRepository.Object);
        }

        [Test]
        public void ThenNewOrgIsNotCreated()
        {
            _orgRepos.Setup(r => r.CreateNewOrganisation(It.IsAny<CreateOrganisationDomainModel>()))
                .Throws(new Exception("Should not be called"));
            _handler.Handle(new CreateOrganisationRequest(){EndPointAssessorOrganisationId = "12345"}, new CancellationToken()).Wait();
        }

        [Test]
        public void ThenExistingOrgIsUpdated()
        {
            _handler.Handle(new CreateOrganisationRequest(){ EndPointAssessorOrganisationId = "12345" }, new CancellationToken()).Wait();
            _orgRepos.Verify(r =>
                r.UpdateOrganisation(
                    It.Is<UpdateOrganisationDomainModel>(m => m.EndPointAssessorOrganisationId == "12345")));
        }
    }
}
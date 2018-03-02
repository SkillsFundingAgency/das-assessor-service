using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Domain;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.OrganisationHandlers;


namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Post.Handlers
{
    using AssessorService.Domain.Consts;

    [TestFixture]
    public class WhenDeletedOrganisationExists
    {
        private CreateOrganisationHandler _handler;
        private Mock<IOrganisationRepository> _orgRepos;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            _orgRepos = new Mock<IOrganisationRepository>();
            
            var orgQueryRepos = new Mock<IOrganisationQueryRepository>();
            orgQueryRepos.Setup(r => r.GetByUkPrn(It.IsAny<int>())).ReturnsAsync(new Organisation()
            {
                Status = OrganisationStatus.Deleted,
                EndPointAssessorOrganisationId = "12345"
            });

            _orgRepos.Setup(r => r.UpdateOrganisation(It.IsAny<OrganisationUpdateDomainModel>()))
                .ReturnsAsync(new Organisation());

            _handler = new CreateOrganisationHandler(_orgRepos.Object, orgQueryRepos.Object);
        }

        [Test]
        public void ThenNewOrgIsNotCreated()
        {
            _orgRepos.Setup(r => r.CreateNewOrganisation(It.IsAny<OrganisationCreateDomainModel>()))
                .Throws(new Exception("Should not be called"));
            _handler.Handle(new CreateOrganisationRequest(){EndPointAssessorOrganisationId = "12345"}, new CancellationToken()).Wait();
        }

        [Test]
        public void ThenExistingOrgIsUpdated()
        {
            _handler.Handle(new CreateOrganisationRequest(){ EndPointAssessorOrganisationId = "12345" }, new CancellationToken()).Wait();
            _orgRepos.Verify(r =>
                r.UpdateOrganisation(
                    It.Is<OrganisationUpdateDomainModel>(m => m.EndPointAssessorOrganisationId == "12345")));
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessmentOrgs.Api.Client.Core.Types;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    [TestFixture]
    public class WhenRegisterUpdateRequestIsSent
    {
        private RegisterUpdateHandler _registerUpdateHandler;
        private Mock<IAssessmentOrgsApiClient> _apiClient;
        private Mock<IOrganisationRepository> _organisationRepository;

        [SetUp]
        public void Arrange()
        {
            _apiClient = new Mock<IAssessmentOrgsApiClient>();
            _organisationRepository = new Mock<IOrganisationRepository>();
            _registerUpdateHandler = new RegisterUpdateHandler(_apiClient.Object, _organisationRepository.Object);
        }


        [Test]
        public void ACallToTheEpaoRegisterApiIsMade()
        {
            _registerUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            _apiClient.Verify(client => client.FindAllAsync()); 
        }

        [Test]
        public void ThenAllOfTheOrganisationsAreRequested()
        {
            _registerUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            _organisationRepository.Verify(repo => repo.GetAllOrganisations());
        }

        [Test]
        public void AndNewEPAODetected_ThenDetailsAreRequestedForNewEPAO()
        {
            _apiClient.Setup(c => c.FindAllAsync()).ReturnsAsync(new List<OrganisationSummary>()
            {
                new OrganisationSummary() {Id = "EPA0001", Name = "EPAO1 Name"},
                new OrganisationSummary() {Id = "EPA0002", Name = "EPAO2 Name"}
            });

            _apiClient.Setup(c => c.Get("EPA0002")).Returns(new AssessmentOrgs.Api.Client.Core.Types.Organisation
            {
                Id = "EPA0002",
                Name = "A new Organisation"
            });

            _organisationRepository.Setup(r => r.GetAllOrganisations()).ReturnsAsync(new List<Organisation>()
            {
                new Organisation()
                {
                    EpaoOrgId = "EPA0001"
                }
            });

            _registerUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            _apiClient.Verify(c => c.Get("EPA0002"));
        }

        [Test]
        public void ThenNewEPAOIsInserted()
        {
            _apiClient.Setup(c => c.FindAllAsync()).ReturnsAsync(new List<OrganisationSummary>()
            {
                new OrganisationSummary() {Id = "EPA0001", Name = "EPAO1 Name"},
                new OrganisationSummary() {Id = "EPA0002", Name = "EPAO2 Name"}
            });

            _apiClient.Setup(c => c.Get("EPA0002")).Returns(new AssessmentOrgs.Api.Client.Core.Types.Organisation
            {
                Id = "EPA0002",
                Name = "A new Organisation"
            });

            _organisationRepository.Setup(r => r.GetAllOrganisations()).ReturnsAsync(new List<Organisation>()
            {
                new Organisation()
                {
                    EpaoOrgId = "EPA0001"
                }
            });

            _registerUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            _organisationRepository.Verify(r =>
                r.CreateNewOrganisation(It.Is<Organisation>(c =>
                    c.EpaoOrgId == "EPA0002" && c.Name == "A new Organisation")));
        }
    }
}
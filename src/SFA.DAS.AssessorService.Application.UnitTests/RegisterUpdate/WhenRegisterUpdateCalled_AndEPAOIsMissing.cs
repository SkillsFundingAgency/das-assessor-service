using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessmentOrgs.Api.Client.Core.Types;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    [TestFixture]
    public class WhenRegisterUpdateCalled_AndEPAOIsMissing : RegisterUpdateTestsBase
    {
        private Guid _organisationId;

        [SetUp]
        public void Arrange()
        {
            Setup();

            ApiClient.Setup(c => c.FindAllAsync())
                .Returns(Task.FromResult(new List<OrganisationSummary>()
                {
                    new OrganisationSummary {Id = "EPA0001"}
                    
                }.AsEnumerable()));

            _organisationId = Guid.NewGuid();
            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<OrganisationQueryViewModel>
                {
                    new OrganisationQueryViewModel() {EndPointAssessorOrganisationId = "EPA0001"},
                    new OrganisationQueryViewModel() {EndPointAssessorOrganisationId = "EPA0002", Id = _organisationId}
                }.AsEnumerable()));
        }

        [Test]
        public void ThenTheRepositoryIsAskedToDeleteTheCorrectOrganisation()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            Mediator.Verify(m => m.Send(It.Is<OrganisationDeleteViewModel>(vm => vm.Id == _organisationId), default(CancellationToken)));
            //OrganisationRepository.Verify(r => r.Delete(_organisationId));//.DeleteOrganisationByEpaoId("EPA0002"));
        }
    }
}
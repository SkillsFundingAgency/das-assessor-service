using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.RegisterUpdate
{


    [TestFixture]
    public class WhenRegisterUpdateCalled_AndEPAOIsMissing : RegisterUpdateTestsBase
    {
        private string _endPointAssessorOrganisationId;

        [SetUp]
        public void Arrange()
        {
            Setup();

            ApiClient.Setup(c => c.FindAllAsync())
                .Returns(Task.FromResult(new List<OrganisationSummary>()
                {
                    new OrganisationSummary {Id = "EPA0001", Ukprn = 11111111}
                    
                }.AsEnumerable()));

            _endPointAssessorOrganisationId = "EPA0002";
            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<Organisation>
                {
                    new Organisation() {EndPointAssessorOrganisationId = "EPA0001", EndPointAssessorUkprn = 11111111},
                    new Organisation() {EndPointAssessorOrganisationId = "EPA0002", Status = OrganisationStatus.Live, EndPointAssessorUkprn = 22222222}
                }.AsEnumerable()));
        }

        [Test]
        public void ThenTheRepositoryIsAskedToDeleteTheCorrectOrganisation()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            Mediator.Verify(m => m.Send(It.Is<DeleteOrganisationRequest>(vm => vm.EndPointAssessorOrganisationId == _endPointAssessorOrganisationId), default(CancellationToken)));
            //OrganisationRepository.Verify(r => r.Delete(_endPointAssessorOrganisationId));//.DeleteOrganisationByEpaoId("EPA0002"));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.RegisterUpdate
{


    public class WhenRegisterUpdateCalled_AndUkPrnIsDifferent : RegisterUpdateTestsBase
    {
            private string _endPointAssessorOrganisationId;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _endPointAssessorOrganisationId = "1234";

            ApiClient.Setup(c => c.FindAllAsync())
                .Returns(Task.FromResult(new List<OrganisationSummary>()
                {
                    new OrganisationSummary { Id = _endPointAssessorOrganisationId, Name = "OLD NAME", Ukprn = 11111111},
                    new OrganisationSummary { Id = "EPA0002", Name = "Another EPAO", Ukprn = 99999999}

                }.AsEnumerable()));


            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<Organisation>
                {
                    new Organisation() { EndPointAssessorOrganisationId = _endPointAssessorOrganisationId, EndPointAssessorName = "OLD NAME", EndPointAssessorUkprn = 22222222},
                    new Organisation() { EndPointAssessorOrganisationId = "EPA0002", EndPointAssessorName = "Another EPAO", EndPointAssessorUkprn = 99999999}
                }.AsEnumerable()));
        }

        [Test]
        public void ThenOrganisationIsUpdatedWithNewUkPrn()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            Mediator.Verify(m =>
                m.Send(
                    It.Is<UpdateOrganisationRequest>(vm =>
                        vm.EndPointAssessorOrganisationId == _endPointAssessorOrganisationId && vm.EndPointAssessorUkprn == 11111111),
                    new CancellationToken()));
        }
    }
}
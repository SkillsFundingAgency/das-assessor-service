using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.RegisterUpdate
{
    [TestFixture]
    public class WhenRegisterUpdateCalled_AndItReturnsUnknownEpao : RegisterUpdateTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            ApiClient.Setup(c => c.FindAllAsync())
                .Returns(Task.FromResult(new List<OrganisationSummary>()
                {
                    new OrganisationSummary {Id = "EPA0001", Ukprn = 11111111},
                    new OrganisationSummary {Id = "EPA0002", Ukprn = 22222222},
                    new OrganisationSummary {Id = "EPA0003", Ukprn = 33333333}
                }.AsEnumerable()));

            ApiClient.Setup(c => c.Get("EPA0003")).Returns(new ExternalApis.AssessmentOrgs.Types.Organisation { Id = "EPA0003", Name = "A New EPAO", UkPrn = 33333333});

            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<Organisation>
                {
                    new Organisation() { EndPointAssessorOrganisationId = "EPA0001", EndPointAssessorUkprn = 11111111},
                    new Organisation() { EndPointAssessorOrganisationId = "EPA0002", EndPointAssessorUkprn = 22222222}
                }.AsEnumerable()));
        }

        [Test]
        public void ThenTheApiIsAskedForMoreDetails()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            ApiClient.Verify(c => c.Get("EPA0003"));
        }

        [Test]
        public void ThenTheRepositoryIsAskedToCreateANewOrganisation()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            Mediator.Verify(m =>
                m.Send(
                    It.Is<CreateOrganisationRequest>(vm =>
                        vm.EndPointAssessorOrganisationId == "EPA0003" && vm.EndPointAssessorName == "A New EPAO"),
                    default(CancellationToken)));
        }
    }
}
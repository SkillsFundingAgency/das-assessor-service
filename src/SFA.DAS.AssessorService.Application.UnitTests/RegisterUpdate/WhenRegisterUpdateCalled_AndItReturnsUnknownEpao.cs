using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using System.Collections.Generic;
using System.Linq;
using Moq;

using SFA.DAS.AssessorService.ViewModel.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
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
                    new OrganisationSummary {Id = "EPA0001"},
                    new OrganisationSummary {Id = "EPA0002"},
                    new OrganisationSummary {Id = "EPA0003"}
                }.AsEnumerable()));

            ApiClient.Setup(c => c.Get("EPA0003")).Returns(new ExternalApis.AssessmentOrgs.Types.Organisation { Id = "EPA0003", Name = "A New EPAO" });

            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<AssessorService.Api.Types.Organisation>
                {
                    new AssessorService.Api.Types.Organisation() { EndPointAssessorOrganisationId = "EPA0001" },
                    new AssessorService.Api.Types.Organisation() { EndPointAssessorOrganisationId = "EPA0002"}
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
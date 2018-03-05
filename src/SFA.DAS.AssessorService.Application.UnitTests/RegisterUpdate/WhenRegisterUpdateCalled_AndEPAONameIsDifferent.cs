using SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

    using AssessorService.Api.Types.Models;

    public class WhenRegisterUpdateCalled_AndEPAONameIsDifferent : RegisterUpdateTestsBase
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
                    new OrganisationSummary { Id = _endPointAssessorOrganisationId, Name = "The New EPAO Name"},
                    new OrganisationSummary { Id = "EPA0002", Name = "Another EPAO"}

                }.AsEnumerable()));


            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<AssessorService.Api.Types.Models.Organisation>
                {
                    new AssessorService.Api.Types.Models.Organisation() { EndPointAssessorOrganisationId = _endPointAssessorOrganisationId, EndPointAssessorName = "OLD NAME" },
                    new AssessorService.Api.Types.Models.Organisation() { EndPointAssessorOrganisationId = "EPA0002", EndPointAssessorName = "Another EPAO"}
                }.AsEnumerable()));
        }

        [Test]
        public void ThenOrganisationIsUpdatedWithNewName()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            Mediator.Verify(m =>
                m.Send(
                    It.Is<UpdateOrganisationRequest>(vm =>
                        vm.EndPointAssessorOrganisationId == _endPointAssessorOrganisationId && vm.EndPointAssessorName == "The New EPAO Name"),
                    new CancellationToken()));
        }
    }
}
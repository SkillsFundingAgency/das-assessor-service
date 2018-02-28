using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    using AssessorService.Api.Types.Models;

    public class WhenRegisterUpdateCalled_AndEPAONameIsDifferent : RegisterUpdateTestsBase
    {
        private Guid _organisationId;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisationId = Guid.NewGuid();

            ApiClient.Setup(c => c.FindAllAsync())
                .Returns(Task.FromResult(new List<OrganisationSummary>()
                {
                    new OrganisationSummary {Id = "EPA0001", Name = "The New EPAO Name"},
                    new OrganisationSummary {Id = "EPA0002", Name = "Another EPAO"}

                }.AsEnumerable()));


            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<Organisation>
                {
                    new Organisation() {EndPointAssessorOrganisationId = "EPA0001", EndPointAssessorName = "OLD NAME", Id = _organisationId},
                    new Organisation() {EndPointAssessorOrganisationId = "EPA0002", EndPointAssessorName = "Another EPAO"}
                }.AsEnumerable()));
        }

        [Test]
        public void ThenOrganisationIsUpdatedWithNewName()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            Mediator.Verify(m =>
                m.Send(
                    It.Is<UpdateOrganisationRequest>(vm =>
                        vm.Id == _organisationId && vm.EndPointAssessorName == "The New EPAO Name"),
                    new CancellationToken()));
        }
    }
}
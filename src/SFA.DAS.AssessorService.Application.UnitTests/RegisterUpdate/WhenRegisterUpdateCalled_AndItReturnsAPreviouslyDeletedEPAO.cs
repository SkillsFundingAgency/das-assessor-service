using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.AssessmentOrgs.Api.Client.Core.Types;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SFA.DAS.AssessorService.Domain.Enums;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    [TestFixture]
    public class WhenRegisterUpdateCalledAndItReturnsAPreviouslyDeletedEpao : RegisterUpdateTestsBase
    {
        private Guid _organisationId;

        [SetUp]
        public void Arrange()
        {
            Setup();
            ApiClient.Setup(c => c.FindAllAsync())
                .Returns(Task.FromResult(new List<OrganisationSummary>()
                {
                    new OrganisationSummary {Id = "EPA0001"},
                    new OrganisationSummary {Id = "EPA0002"}
                }.AsEnumerable()));

            //ApiClient.Setup(c => c.Get("EPA0003")).Returns(new Organisation { Id = "EPA0003", Name = "A New EPAO" });

            _organisationId = Guid.NewGuid();
            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<OrganisationQueryViewModel>
                {
                    new OrganisationQueryViewModel() {Id = _organisationId, EndPointAssessorOrganisationId = "EPA0001", Status = OrganisationStatus.Deleted},
                    new OrganisationQueryViewModel() {EndPointAssessorOrganisationId = "EPA0002"}
                }.AsEnumerable()));
        }

        [Test]
        public void ThenTheRepositoryIsAskedToUpdateTheUndeletedOrganisation()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            Mediator.Verify(m =>
                m.Send(
                    It.Is<OrganisationUpdateViewModel>(vm =>
                        vm.Id == _organisationId && vm.OrganisationStatus == OrganisationStatus.New),
                    default(CancellationToken)));
        }
    }
}
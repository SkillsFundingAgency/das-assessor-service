using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SFA.DAS.AssessorService.Domain.Enums;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    using AssessorService.Api.Types.Models;

    [TestFixture]
    public class WhenRegisterUpdateCalledAndItReturnsAPreviouslyDeletedEpao : RegisterUpdateTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            ApiClient.Setup(c => c.FindAllAsync())
                .Returns(Task.FromResult(new List<OrganisationSummary>()
                {
                    new OrganisationSummary {EndPointAssessorOrganisationId = "EPA0001"},
                    new OrganisationSummary {EndPointAssessorOrganisationId = "EPA0002"}
                }.AsEnumerable()));

            //ApiClient.Setup(c => c.Get("EPA0003")).Returns(new Organisation { Id = "EPA0003", Name = "A New EPAO" });

            OrganisationRepository.Setup(r => r.GetAllOrganisations())
                .Returns(Task.FromResult(new List<Organisation>
                {
                    new Organisation() { EndPointAssessorOrganisationId = "EPA0001",  OrganisationStatus = OrganisationStatus.Deleted},
                    new Organisation() { EndPointAssessorOrganisationId = "EPA0002"}
                }.AsEnumerable()));
        }

        [Test]
        [Ignore("Test temporarily not needed until change to functionality")]
        public void ThenTheRepositoryIsAskedToUpdateTheUndeletedOrganisation()
        {
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            Assert.Fail("This now needs to verify that a CREATE is done with the correct values");

            //Mediator.Verify(m =>
            //    m.Send(
            //        It.Is<UpdateOrganisationRequest>(vm =>
            //            vm.EndPointAssessorOrganisationId == "EPA0001" && vm.OrganisationStatus == OrganisationStatus.New),
            //        default(CancellationToken)));
        }
    }
}
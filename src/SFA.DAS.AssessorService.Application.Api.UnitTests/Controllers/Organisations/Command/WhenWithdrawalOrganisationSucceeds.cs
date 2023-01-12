using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Command
{
    public class WhenWithdrawalOrganisationSucceeds : OrganisationTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
        }

        [Test]
        public void ThenAResultShouldBeReturned()
        {
            var request = new WithdrawOrganisationRequest();

            var result = OrganisationController.WithdrawalOrganisation(request).Result as NoContentResult;

            result.Should().NotBeNull();

            Mediator.Verify(q => q.Send(request, Moq.It.IsAny<CancellationToken>()));
        }
    }
}

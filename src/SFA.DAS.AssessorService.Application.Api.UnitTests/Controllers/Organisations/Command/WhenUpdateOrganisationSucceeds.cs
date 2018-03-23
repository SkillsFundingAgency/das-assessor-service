using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Command
{
    public class WhenUpdateOrganisationSucceeds : OrganisationTestBase
    {
        private UpdateOrganisationRequest _updateOrganisationRequest;
        private  Organisation _organisationResponse;
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisationResponse = Builder<Organisation>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<UpdateOrganisationRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_organisationResponse)));

            _updateOrganisationRequest = Builder<UpdateOrganisationRequest>.CreateNew()
                    .Build();
            _result = OrganisationController.UpdateOrganisation(_updateOrganisationRequest).Result;
        }

        [Test]
        public void ThenAResultShouldBeReturned()
        {
            var result = _result as NoContentResult;
            result.Should().NotBeNull();
        }
    }
}

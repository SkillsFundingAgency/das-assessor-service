using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Put.Controller
{
    public class WhenUpdateOrganisationSucceeds : OrganisationTestBase
    {
        private UpdateOrganisationRequest _updateOrganisationRequest;
        private  OrganisationResponse _organisationResponse;
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisationResponse = Builder<OrganisationResponse>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<UpdateOrganisationRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_organisationResponse)));

            _updateOrganisationRequest = Builder<UpdateOrganisationRequest>.CreateNew()
                    .Build();
            _result = OrganisationController.UpdateOrganisation(_updateOrganisationRequest).Result;
        }

        [Test]
        public void ThenAResultShouldBeReturned()
        {
            var result = _result as Microsoft.AspNetCore.Mvc.NoContentResult;
            result.Should().NotBeNull();
        }
    }
}

using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Command
{
    public class WhenCreateOrganisation : OrganisationTestBase
    {
        private static Organisation _organisation;
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisation = Builder<Organisation>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<CreateOrganisationRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_organisation)));

            _result = OrganisationController.CreateOrganisation(default(CreateOrganisationRequest)).Result;
        }

        [Test]
        public void ThenAResultShouldBeReturned()
        {
            var result = _result as CreatedAtRouteResult;
            result.Should().NotBeNull();
        }
    }
}

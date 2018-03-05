using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Post.Controller
{   
    public class WhenCreateOrganisation : OrganisationTestBase
    {
        private static CreateOrganisationRequest _organisationCreateViewModel;
        private static Organisation _organisationQueryViewModel;
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisationQueryViewModel = Builder<Organisation>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<CreateOrganisationRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_organisationQueryViewModel)));

            _result = OrganisationController.CreateOrganisation(_organisationCreateViewModel).Result;
        }

        [Test]
        public void ThenAResultSHouldBeReturned()
        { 
            var result = _result as Microsoft.AspNetCore.Mvc.CreatedAtRouteResult;
            result.Should().NotBeNull();
        }
    }
}

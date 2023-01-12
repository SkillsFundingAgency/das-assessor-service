using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    public class OrganisationControllerTestBaseForModel<T> : OrganisationControllerTestBase
    {
        [Test]
        public async Task Should_get_an_organisation_by_epao()
        {
            _actionResult = await Act();
            OrganisationApiClient.Verify(a => a.GetEpaOrganisation(EpaoId));
        }

        [Test]
        public async Task Should_return_a_viewresult()
        {
            _actionResult = await Act();
            _actionResult.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task Should_return_a_model()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            result.Model.Should().BeOfType<T>();
        }

        public virtual async Task<IActionResult> Act()
        {
            return await Task.FromResult<IActionResult>(null);
        }

        public virtual async Task<IActionResult> Act(T viewModel)
        {
            return await Task.FromResult<IActionResult>(null);
        }
    }
}

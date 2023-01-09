using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangeAddressWithoutErrors : OrganisationControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);
        }

        public async Task<IActionResult> Act()
        {
            return await sut.ChangeAddress();
        }

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
            result.Model.Should().BeOfType<ChangeAddressViewModel>();
        }

        [Test]
        public async Task Should_return_a_model_with_current_address()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangeAddressViewModel;

            model.AddressLine1.Should().Be(EpaOrganisation.OrganisationData?.Address1);
            model.AddressLine2.Should().Be(EpaOrganisation.OrganisationData?.Address2);
            model.AddressLine3.Should().Be(EpaOrganisation.OrganisationData?.Address3);
            model.AddressLine4.Should().Be(EpaOrganisation.OrganisationData?.Address4);
            model.Postcode.Should().Be(EpaOrganisation.OrganisationData?.Postcode);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangeAddressConfirm
        : OrganisationControllerTestBaseForModel<ChangeAddressViewModel>
    {
        private const string ValidAddressLine1Different = "DIFFERENT";
        private const string ValidAddressLine2Different = "DIFFERENT";
        private const string ValidAddressLine3Different = "DIFFERENT";
        private const string ValidAddressLine4Different = "DIFFERENT";
        private const string ValidPostcodeDifferent = "DIFFERENT";
        private const string ActionChoiceConfirm = "Confirm";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            OrganisationApiClient.Setup(c => c.UpdateEpaOrganisationAddress(It.IsAny<UpdateEpaOrganisationAddressRequest>()))
                .ReturnsAsync(new List<ContactResponse>());
        }

        public override async Task<IActionResult> Act()
        {            
            return await sut.ChangeAddress(new ChangeAddressViewModel
            {
                AddressLine1 = ValidAddressLine1Different,
                AddressLine2 = ValidAddressLine2Different,
                AddressLine3 = ValidAddressLine3Different,
                AddressLine4 = ValidAddressLine4Different,
                Postcode = ValidPostcodeDifferent,
                ActionChoice = ActionChoiceConfirm
            });
        }

        public override async Task<IActionResult> Act(ChangeAddressViewModel viewModel)
        {
            return await sut.ChangeAddress(viewModel);
        }

        [TestCase("DIFFERENT", ValidAddress2, ValidAddress3, ValidAddress4, ValidPostcode)]
        [TestCase(ValidAddress1, "DIFFERENT", ValidAddress3, ValidAddress4, ValidPostcode)]
        [TestCase(ValidAddress1, ValidAddress2, "DIFFERENT", ValidAddress4, ValidPostcode)]
        [TestCase(ValidAddress1, ValidAddress2, ValidAddress3, "DIFFERENT", ValidPostcode)]
        [TestCase(ValidAddress1, ValidAddress2, ValidAddress3, ValidAddress4, "DIFFERENT")]
        public async Task Should_update_address(string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postcode)
        {
            _actionResult = await Act(new ChangeAddressViewModel
            {
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                AddressLine3 = addressLine3,
                AddressLine4 = addressLine4,
                Postcode = postcode,
                ActionChoice = ActionChoiceConfirm
            });

            OrganisationApiClient.Verify(c => c.UpdateEpaOrganisationAddress(
                It.Is<UpdateEpaOrganisationAddressRequest>(p => 
                    p.OrganisationId == EpaoId && 
                    p.AddressLine1 == addressLine1 &&
                    p.AddressLine2 == addressLine2 &&
                    p.AddressLine3 == addressLine3 &&
                    p.AddressLine4 == addressLine4 &&
                    p.Postcode == postcode)), 
                Times.Once);
        }

        [Test]
        public async Task Should_return_a_viewresult_called_updated()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("ChangeAddressUpdated");
        }
    }
}

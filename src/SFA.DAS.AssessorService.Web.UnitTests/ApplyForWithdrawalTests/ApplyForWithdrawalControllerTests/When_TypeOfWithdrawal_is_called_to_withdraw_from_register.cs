using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_TypeOfWithdrawal_is_called_to_withdraw_from_register : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public async Task Then_BuildOrganisationWithdrawalRequest_is_called()
        {
            // Act
            await _sut.TypeOfWithdrawal(new TypeOfWithdrawalViewModel { TypeOfWithdrawal = ApplicationTypes.OrganisationWithdrawal });

            // Assert
            _mockApplicationService
                .Verify(r => r.BuildOrganisationWithdrawalRequest(It.IsAny<ContactResponse>(), It.IsAny<OrganisationResponse>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Then_CreateApplication_is_called()
        {
            // Act
            await _sut.TypeOfWithdrawal(new TypeOfWithdrawalViewModel { TypeOfWithdrawal = ApplicationTypes.OrganisationWithdrawal });

            // Assert
            _mockApplicationApiClient
                .Verify(r => r.CreateApplication(It.IsAny<CreateApplicationRequest>()), Times.Once);
        }

        [Test]
        public async Task Then_Redirect_To_Sequence()
        {
            // Act
            var result = await _sut.TypeOfWithdrawal(new TypeOfWithdrawalViewModel { TypeOfWithdrawal = ApplicationTypes.OrganisationWithdrawal }) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(nameof(ApplicationController.Sequence));
            result.ControllerName.Should().Be(nameof(ApplicationController).RemoveController());

            result.RouteValues.TryGetValue("sequenceNo", out object organisationWithdrawalSequenceNo).Should().BeTrue();
            ((int)organisationWithdrawalSequenceNo).Should().Be(ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO);
        }
    }
}
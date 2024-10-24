using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Web.Helpers;
using FluentAssertions;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    public class OfsShutterPageSelectorTests
    {
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowNeedToRegisterPage_IsSet_ToOppositeOfOfsStatus(bool isOfs, bool expected)
        {
            var organisation = new EpaOrganisation();
            var standard = new AppliedStandardVersion();
            var mockOrgApiClient = new Mock<IOrganisationsApiClient>();
            mockOrgApiClient.Setup(m => m.IsOfsOrganisation(organisation))
                            .ReturnsAsync(isOfs);

            var sut = new OfsShutterPageSelector(mockOrgApiClient.Object);
            var result = await sut.GetFromOrganisationAndStandard(organisation, standard);

            result.ShowNeedToRegisterPage.Should().Be(expected);
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public async Task ShowNeedToSubmitIlrPage_IsSet(bool isOfs, bool requiresApproval, bool expected)
        {
            var organisation = new EpaOrganisation();
            var standard = new AppliedStandardVersion() { EpaoMustBeApprovedByRegulatorBody = requiresApproval};
            var mockOrgApiClient = new Mock<IOrganisationsApiClient>();
            mockOrgApiClient.Setup(m => m.IsOfsOrganisation(organisation))
                            .ReturnsAsync(isOfs);

            var sut = new OfsShutterPageSelector(mockOrgApiClient.Object);
            var result = await sut.GetFromOrganisationAndStandard(organisation, standard);

            result.ShowNeedToSubmitIlrPage.Should().Be(expected);
        }
    }
}

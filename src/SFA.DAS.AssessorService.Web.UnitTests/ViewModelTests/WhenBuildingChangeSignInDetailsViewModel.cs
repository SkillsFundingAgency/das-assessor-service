using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.UnitTests.ViewModelTests;

public class WhenBuildingChangeSignInDetailsViewModel
{
    [Test]
    public void Then_The_Url_Is_Correctly_Set_For_Production()
    {
        var actual = new ChangeSignInDetailsViewModel("PrD");

        actual.SettingsLink.Should().Be("https://home.account.gov.uk/settings");
    }
    [Test]
    public void Then_The_Url_Is_Correctly_Set_For_Non_Production()
    {
        var actual = new ChangeSignInDetailsViewModel("test");

        actual.SettingsLink.Should().Be("https://home.integration.account.gov.uk/settings");
    }
}
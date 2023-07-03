using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels.Dashboard;

namespace SFA.DAS.AssessorService.Web.UnitTests.ViewModelTests.DashboardViewModelTests;

public class WhenCallingBannerViewPath
{
    [Test]
    public void ThenBannerViewPathConstantIsReturned()
    {
        var sut = new DashboardViewModel();
        Assert.That(sut.BannerViewPath, Is.EqualTo(Constants.Banners.NoLongerAssessBannerViewPath));
    }
}

using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Models;

namespace SFA.DAS.AssessorService.Web.UnitTests.ModelsTests.HomeIndexViewModelTests;

public class WhenCallingBannerViewPath
{
    [Test]
    public void ThenBannerViewPathConstantIsReturned()
    {
        var sut = new HomeIndexViewModel();
        Assert.That(sut.BannerViewPath, Is.EqualTo(Constants.Banners.NoLongerAssessBannerViewPath));
    }
}

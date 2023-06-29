using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Helpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers;

public class BannerTests
{
    [Test, MoqAutoData]
    public void ViewPath_ReturnsValueSetInConstructor(string path)
    {
        var sut = new Banner(path);
        Assert.That(sut.ViewPath, Is.EqualTo(path));
    }
}

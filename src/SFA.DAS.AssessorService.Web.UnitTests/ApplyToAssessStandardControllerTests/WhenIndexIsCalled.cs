using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyToAssessStandardControllerTests;

public class WhenIndexIsCalled
{
    [Test]
    public void RedirectedToDashboardIndex()
    {
        var sut = new ApplyToAssessStandardController();
        var result = sut.Index();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            Assert.That(result.As<RedirectToActionResult>().ControllerName, Is.EqualTo("Dashboard"));
            Assert.That(result.As<RedirectToActionResult>().ActionName, Is.EqualTo("Index"));
        });
    }
}

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    public class ApplyStandardOfsShutterPageTests : StandardControllerTestBase
    {
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void ViewModelProperties_SetToParameters(bool showNeedToRegisterPage, bool showNeedToSubmitIlrPage)
        {
            var result = _sut.ApplyStandardOfsShutterPage(Guid.NewGuid(), "doesn't matter", showNeedToRegisterPage, showNeedToSubmitIlrPage) as ViewResult;
            var vm = result.Model as ApplyStandardOfsShutterPageViewModel;

            using (new AssertionScope())
            {
                vm.ShowNeedToRegisterPage.Should().Be(showNeedToRegisterPage);
                vm.ShowNeedToSubmitIlrPage.Should().Be(showNeedToSubmitIlrPage);
            }
        }
    }
}

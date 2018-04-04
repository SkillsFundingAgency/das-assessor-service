using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class DontHaveAnAccountPage : BasePage
    {
        private static string PAGE_TITLE = "Registration";

        public DontHaveAnAccountPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }
    }
}
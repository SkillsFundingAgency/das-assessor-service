using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class DeclarationtPage : BasePage
    {
        private static string PAGE_TITLE = "Declaration";

        public DeclarationtPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By _confirmAndAccept = By.XPath("//*[@id=\"content\"]/div/div/form/button");

        internal void ClickConfirmAndAccept()
        {
            FormCompletionHelper.ClickElement(_confirmAndAccept);
        }
    }
}
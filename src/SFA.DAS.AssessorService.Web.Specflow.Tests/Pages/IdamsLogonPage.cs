using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class IdamsLogonPage : BasePage
    {
        private static String PAGE_TITLE = "";

        public IdamsLogonPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By _startNowButton = By.Id("bySelection");

        internal void SelectProvider()
        {
            var element = webDriver.FindElement(By.XPath("//span[contains(@class,'largeTextNoWrap')]  [contains(text(),'Pirean Preprod')]"));
            element.Click();
        }
    }
}
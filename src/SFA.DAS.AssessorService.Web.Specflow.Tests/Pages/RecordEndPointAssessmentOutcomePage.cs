using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class RecordEndPointAssessmentOutcomePage : BasePage
    {
        private static String PAGE_TITLE = "Record end-point assessment outcome";

        public RecordEndPointAssessmentOutcomePage(IWebDriver webDriver) : base(webDriver)
        {
            PageInteractionHelper.WaitForPageToLoad();

            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _startNowButton = By.CssSelector(".button-start");

        internal IdamsLogonPage ClickStartNowButton()
        {
            FormCompletionHelper.ClickElement(_startNowButton);
            return new IdamsLogonPage(webDriver);
        }
    }
}
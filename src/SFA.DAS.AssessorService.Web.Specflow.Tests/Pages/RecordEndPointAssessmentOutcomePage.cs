using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class RecordEndPointAssessmentOutcomePage : BasePage
    {
        private static string PAGE_TITLE = "Record apprentice end-point assessment grades";

        public RecordEndPointAssessmentOutcomePage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _startNowButton = By.CssSelector(".button-start");

        internal void ClickStartNowButton()
        {
            FormCompletionHelper.ClickElement(_startNowButton);
        }
    }
}
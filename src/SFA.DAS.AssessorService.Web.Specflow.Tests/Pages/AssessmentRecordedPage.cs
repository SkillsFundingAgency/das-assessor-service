using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class AssessmentRecodedPage : BasePage
    {
        private static string PAGE_TITLE = "Assessment recorded";

        public AssessmentRecodedPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _signOff = By.XPath("//*[@id=\"content\"]/div/div/ul/li[3]/a");

        internal void ClickSignOut()
        {
            FormCompletionHelper.ClickElement(_signOff);
        }
    }
}
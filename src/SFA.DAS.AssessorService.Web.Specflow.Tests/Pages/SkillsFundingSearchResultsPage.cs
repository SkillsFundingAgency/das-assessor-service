using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class SkillsFundingSearchResultsPage : BasePage
    {
        private static String PAGE_TITLE = "Search for an apprentice";

        public SkillsFundingSearchResultsPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By _dfeLink = By.LinkText("Department for Education");

        internal void ClickDfeLink()
        {
            FormCompletionHelper.ClickElement(_dfeLink);
        }
    }
}
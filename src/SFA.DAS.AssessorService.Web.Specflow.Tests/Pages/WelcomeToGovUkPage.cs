using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class WelcomeToGovUkPage : BasePage
    {
        private static String PAGE_TITLE = "Welcome to GOV.UK";

        public WelcomeToGovUkPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _searchField = By.Name("q");
        private readonly By _searchButton = By.CssSelector(".search-submit");

        internal SearchResultsPage EnterSearchTextAndSubmit(String searchText)
        {
            FormCompletionHelper.EnterText(_searchField, searchText);
            FormCompletionHelper.ClickElement(_searchButton);
            return new SearchResultsPage(WebDriver);
        }
    }
}
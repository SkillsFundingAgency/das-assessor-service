using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class SearchForAnApprenticePage : BasePage
    {
        private static String PAGE_TITLE = "Search for an apprentice";

        public SearchForAnApprenticePage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _dfeLink = By.LinkText("Department for Education");

        internal DepartmentForEducationHomePage ClickDfeLink()
        {
            FormCompletionHelper.ClickElement(_dfeLink);
            return new DepartmentForEducationHomePage(WebDriver);
        }
    }
}
using System;
using System.Threading;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class SkillsFundingLogonPage : BasePage
    {
        private static String PAGE_TITLE = "Sign in";

        public SkillsFundingLogonPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By _userNameField = By.Name("username");
        private By _passwordField = By.Name("password");
        private By _signInButton = By.XPath("//div[contains(@class, 'btn') and contains(@class, 'btn-ml')]");

        internal SkillsFundingSearchResultsPage EnterUserDetails(String userName, string password)
        {

            FormCompletionHelper.EnterText(_userNameField, userName);
            FormCompletionHelper.EnterText(_passwordField, password);

            var element = webDriver.FindElement(By.XPath("//span[contains(text(),'Sign in')]"));
            var parent = element.FindElement(By.XPath(".."));
            parent.Click();

            Thread.Sleep(5000);

            return new SkillsFundingSearchResultsPage(webDriver);
        }

        public ForgottenPaswordPage SelectForgotMyPassword()
        {
            var element = webDriver.FindElement(By.XPath("//a[contains(text(),'I forgot my password')]"));
            element.Click();

            return new ForgottenPaswordPage(webDriver);
        }

        public void SelectDontHaveAnAccount()
        {
            var element = webDriver.FindElement(By.XPath("//a[contains(text(),'have an account')]"));
            element.Click();
        }
    }
}
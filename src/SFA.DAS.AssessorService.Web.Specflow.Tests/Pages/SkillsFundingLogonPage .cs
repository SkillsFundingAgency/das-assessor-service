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

        private readonly By _userNameField = By.Name("username");
        private readonly By _passwordField = By.Name("password");
        private By _signInButton = By.XPath("//div[contains(@class, 'btn') and contains(@class, 'btn-ml')]");

        internal SkillsFundingSearchResultsPage EnterUserDetails(String userName, string password)
        {

            FormCompletionHelper.EnterText(_userNameField, userName);
            FormCompletionHelper.EnterText(_passwordField, password);

            var element = WebDriver.FindElement(By.XPath("//span[contains(text(),'Sign in')]"));
            var parent = element.FindElement(By.XPath(".."));
            parent.Click();

            Thread.Sleep(5000);

            return new SkillsFundingSearchResultsPage(WebDriver);
        }

        public ForgottenPaswordPage SelectForgotMyPassword()
        {
            var element = WebDriver.FindElement(By.XPath("//a[contains(text(),'I forgot my password')]"));
            element.Click();

            return new ForgottenPaswordPage(WebDriver);
        }

        public void SelectDontHaveAnAccount()
        {
            var element = WebDriver.FindElement(By.XPath("//a[contains(text(),'have an account')]"));
            element.Click();
        }
    }
}
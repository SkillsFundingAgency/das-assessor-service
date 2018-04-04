using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport
{
    public abstract class BasePage
    {
        protected IWebDriver WebDriver;
        private By _pageHeading = By.CssSelector("h1");

        public BasePage(IWebDriver webDriver)
        {
            this.WebDriver = webDriver;
            PageFactory.InitElements(webDriver, this);
        }

        protected abstract Boolean SelfVerify();

        protected String GetPageHeading()
        {
            return WebDriver.FindElement(_pageHeading).Text;
        }
    }
}
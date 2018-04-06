using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers
{
    public class PageInteractionHelper
    {
        protected static IWebDriver WebDriver;

        public static void SetDriver(IWebDriver webDriver)
        {
            PageInteractionHelper.WebDriver = webDriver;
        }

        public static Boolean VerifyPageHeading(String actual, String expected)
        {
            if(actual.Contains(expected))
            {
                return true;
            }

            throw new Exception("Page verification failed:"
                + "\n Expected: " + expected + " page"
                + "\n Found: " + actual + " page");
        }

        public static Boolean VerifyPageHeading(By locator, String expected)
        {
            String actual = WebDriver.FindElement(locator).Text;
            if (actual.Contains(expected))
            {
                return true;
            }

            throw new Exception("Page verification failed:"
                + "\n Expected: " + expected + " page"
                + "\n Found: " + actual + " page");
        }

        public static Boolean VerifyPageHeading(String actual, String expected1, String expected2)
        {
            if(actual.Contains(expected1) || actual.Contains(expected2))
            {
                return true;
            }

            throw new Exception("Page verification failed: "
                + "\n Expected: " + expected1 + " or " + expected2 + " pages"
                + "\n Found: " + actual + " page");
        }

        public static Boolean VerifyText(String actual, String expected)
        {
            if (actual.Contains(expected))
            {
                return true;
            }

            throw new Exception("Text verification failed: "
                + "\n Expected: " + expected
                + "\n Found: "+ actual);
        }

        public static Boolean VerifyText(By locator, String expected)
        {
            String actual = WebDriver.FindElement(locator).Text;
            if (actual.Contains(expected))
            {
                return true;
            }

            throw new Exception("Text verification failed: "
                + "\n Expected: " + expected
                + "\n Found: " + actual);
        }

        public static void WaitForPageToLoad(int implicitWaitTime = 10)
        {
            Thread.Sleep(500);
            var waitForDocumentReady = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(implicitWaitTime));
            waitForDocumentReady.Until((wdriver) => (WebDriver as IJavaScriptExecutor).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static void WaitForElementToBePresent(By locator)
        {
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(locator));
        }

        public static void WaitForElementToBeDisplayed(By locator)
        {
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public static Boolean IsElementPresent(By locator)
        {
            TurnOffImplicitWaits();
            try
            {
                WebDriver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            finally
            {
                TurnOnImplicitWaits();
            }
        }

        public static Boolean IsElementDisplayed(By locator)
        {
            TurnOffImplicitWaits();
            try
            {
                return WebDriver.FindElement(locator).Displayed;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                TurnOnImplicitWaits();
            }
        }

        public static void FocusTheElement(By locator)
        {
            IWebElement webElement = WebDriver.FindElement(locator);
            ((IJavaScriptExecutor)WebDriver).ExecuteScript("arguments[0].scrollIntoView(true);", webElement);
            WaitForElementToBeDisplayed(locator);
        }

        public static void TurnOffImplicitWaits()
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
        }

        public static void TurnOnImplicitWaits()
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }
    }
}
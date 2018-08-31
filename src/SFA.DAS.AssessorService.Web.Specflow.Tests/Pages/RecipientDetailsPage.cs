using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class RecipientDetailsPage : BasePage
    {
        private static String PAGE_TITLE = "What is the recipient's name?";

        public RecipientDetailsPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _recipientName = By.Name("Name");
        private readonly By _department = By.Name("Dept");
        private readonly By _continueButton = By.XPath("//*[@id=\"content\"]/div[2]/div/form/button");

        internal void EnterDetails()
        {
            FormCompletionHelper.EnterText(_recipientName, "Test Recipient");
            FormCompletionHelper.EnterText(_department, "Human Resources");
        }

        internal void ClicksContinue()
        {
            FormCompletionHelper.ClickElement(_continueButton);
        }
    }
}
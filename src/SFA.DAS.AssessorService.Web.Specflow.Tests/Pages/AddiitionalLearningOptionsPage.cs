using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class AddiitionalLearningOptionsPage : BasePage
    {
        private static String PAGE_TITLE = "Did the apprentice take a learning option?";

        public AddiitionalLearningOptionsPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _selectedItem = By.Id("options_no");
     
        internal void SelectsNoOption()
        {
            FormCompletionHelper.ClickElement(_selectedItem);
        }

        private readonly By _continueButton = By.XPath("//*[@id=\"content\"]/div/div/form/button");

        internal void ClickContinue()
        {
            FormCompletionHelper.ClickElement(_continueButton);
        }
    }
}
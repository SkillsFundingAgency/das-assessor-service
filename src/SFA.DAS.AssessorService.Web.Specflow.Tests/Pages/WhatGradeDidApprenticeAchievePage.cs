using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class WhatGradeDidApprenticeAchievePage : BasePage
    {
        private static String PAGE_TITLE = "What grade did the apprentice achieve?";

        public WhatGradeDidApprenticeAchievePage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _selectedItem = By.Id("Distinction");
        private readonly By _continueButton = By.XPath("//*[@id=\"content\"]/div/div/form/button");

        internal void SelectsOption()
        {
            FormCompletionHelper.ClickElement(_selectedItem);
        }

        internal void ClicksContinue()
        {
            FormCompletionHelper.ClickElement(_continueButton);
        }
    }
}
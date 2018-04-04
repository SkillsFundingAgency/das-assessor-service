using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class ConfirmApprenticePage : BasePage
    {
        private static String PAGE_TITLE = "Confirm apprentice";

        public ConfirmApprenticePage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By submitButton = By.XPath("//*[@id=\"content\"]/div/div/form/div[2]/button");

        internal WhatGradeDidApprenticeAchievePage ClickStartRecordingAssessment()
        {
            FormCompletionHelper.ClickElement(submitButton);
            return new WhatGradeDidApprenticeAchievePage(webDriver);
        }
    }
}
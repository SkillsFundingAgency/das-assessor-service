﻿using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class AddiitionalLearningOptionsPage : BasePage
    {
        private static String PAGE_TITLE = "Did the apprentice do any additional learning options?";

        public AddiitionalLearningOptionsPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By _continueButton = By.XPath("//*[@id=\"content\"]/div/div/form/button");

        internal ApprenticeAchievementDatePage ClickContinue()
        {
            FormCompletionHelper.ClickElement(_continueButton);
            return new ApprenticeAchievementDatePage(webDriver);
        }
    }
}
﻿using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class ApprenticeAchievementDatePage : BasePage
    {
        private static String PAGE_TITLE = "What is the apprenticeship achievement date?";

        public ApprenticeAchievementDatePage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By _dayBy = By.Id("Day");
        private By _monthy = By.Id("Month");
        private By _yearBy = By.Id("Year");

        private By _continueButton = By.XPath("//*[@id=\"content\"]/div/div/form/button");

        internal void EnterDetails()
        {
            FormCompletionHelper.EnterText(_dayBy, "2");
            FormCompletionHelper.EnterText(_monthy, "3");
            FormCompletionHelper.EnterText(_yearBy, "2018");
        }

        internal WhereWillTheCertificateBeSentPage ClickContinue()
        {
            FormCompletionHelper.ClickElement(_continueButton);
            return new WhereWillTheCertificateBeSentPage(webDriver);
        }
    }
}
using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class AssessmentEnrolledOnMoreThanOneAssessmentPage : BasePage
    {
        private static String PAGE_TITLE = "The apprentice is enrolled on more than one apprenticeship";
        private readonly By _selectedItem = By.Id("standard_93");

        public AssessmentEnrolledOnMoreThanOneAssessmentPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private readonly By _submitButton = By.XPath("//*[@id=\"content\"]/div[2]/div/form/div[3]/button");

        internal void SelectRecordingAssessment()
        {
            FormCompletionHelper.ClickElement(_selectedItem);
        }

        internal void ClickStartRecordingAssessment()
        {
            FormCompletionHelper.ClickElement(_submitButton);
        }
    }
}
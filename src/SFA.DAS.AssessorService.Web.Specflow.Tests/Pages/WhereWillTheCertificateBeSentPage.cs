using System;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Pages
{
    public class WhereWillTheCertificateBeSentPage : BasePage
    {
        private static String PAGE_TITLE = "Where will the certificate be sent?";

        public WhereWillTheCertificateBeSentPage(IWebDriver webDriver) : base(webDriver)
        {
            SelfVerify();
        }

        protected override bool SelfVerify()
        {
            return PageInteractionHelper.VerifyPageHeading(this.GetPageHeading(), PAGE_TITLE);
        }

        private By _nameBy = By.Name("Name");
        private By _departmentBy = By.Name("Dept");
        private By _employerBy = By.Name("Employer");
        private By _postCodeBy = By.Id("postcode-search");

        private By _addressLine1By = By.Name("AddressLine1");
        private By _addressLine2By = By.Name("AddressLine2");
        private By _addressLine3By = By.Name("AddressLine3");
        private By _cityBy = By.Name("City");
        private By _addresspostCode = By.Name("PostCode");

        private By _continueButton = By.XPath("//*[@id=\"content\"]/div/div/form/button");

        internal void EnterDetails()
        {
            FormCompletionHelper.EnterText(_nameBy, "Paul Jones");
            FormCompletionHelper.EnterText(_departmentBy, "Test Deparment");
            FormCompletionHelper.EnterText(_employerBy, "Jongo Ltd");
            FormCompletionHelper.EnterText(_postCodeBy, "B50 3DE");

            FormCompletionHelper.EnterText(_addressLine1By, "1 Anchor Drive");
            FormCompletionHelper.EnterText(_addressLine2By, "Enfield");
            FormCompletionHelper.EnterText(_addressLine3By, "Westminster");
            FormCompletionHelper.EnterText(_cityBy, "London");

            FormCompletionHelper.EnterText(_addresspostCode, "B50 3DE");
        }

        internal CheckAndApproveAssessmentDetails ClickContinue()
        {
            return new CheckAndApproveAssessmentDetails(webDriver);
        }
    }
}
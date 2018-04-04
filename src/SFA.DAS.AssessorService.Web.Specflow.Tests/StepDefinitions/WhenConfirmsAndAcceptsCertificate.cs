﻿using System.Data;
using Dapper;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenConfirmsAndAcceptsCertificate : BaseTest
    {
        private readonly IDbConnection _dbConnection;
        private DeclarationtPage _declarationPage;
        private AssessmentRecodedPage _assessmentRecordedPage;

        public WhenConfirmsAndAcceptsCertificate(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [Then(@"The User is taken to the Declaraton Page")]
        public void ThenTheUserIsTakenToTheDeclaratonPage()
        {
            _declarationPage = new DeclarationtPage(webDriver);
        }

        [When(@"The User Conforms And Applies for a Certificate")]
        public void ThenTheUserConformsAndAppliesForACertificate()
        {
            _declarationPage.ClickConfirmAndAccept();
        }

        private readonly By _certificateNumberBy = By.XPath("//*[@id=\"content\"]/div/div/div/p/strong");

        [Then(@"Assessment Is Recorded")]
        public void ThenAssessmentIsRecorded()
        {
            _assessmentRecordedPage = new AssessmentRecodedPage(webDriver);

            var element = webDriver.FindElement(_certificateNumberBy);
            var certificateNmber = element.Text;

            DeleteCertificate(certificateNmber);

            _assessmentRecordedPage.ClickSignOut();
        }

        private void DeleteCertificate(string certificateNumber)
        {
            var command = $"DELETE FROM Certificates WHERE CertificateReference = {certificateNumber};";
            var result = _dbConnection.Execute(command);
        }
    }
}

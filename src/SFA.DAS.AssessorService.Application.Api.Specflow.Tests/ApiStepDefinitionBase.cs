namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using FluentAssertions;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
    using TechTalk.SpecFlow;
    using System.Threading.Tasks;
    using System.Data;

    [Binding]
    public class ApiStepDefinitionBase
    {
        private readonly RestClient restClient;

        public ApiStepDefinitionBase(RestClient restClient)
        {
            this.restClient = restClient;
        } 

        [BeforeFeature]
        public static void Setup()
        {
            var container = Bootstrapper.Container;
            var database = container.GetInstance<DatabaseUtilities>();

            database.Restore();
        }

        [Given(@"System Has access to the SFA\.DAS\.AssessmentOrgs\.Api")]
        public void GivenSystemHasAccessToTheSFA_DAS_AssessmentOrgs_Api()
        {
            var baseAddress = ConfigurationManager.AppSettings[RestParameters.BaseAddress];

            restClient.HttpClient = new HttpClient();

            restClient.HttpClient.BaseAddress = new Uri(baseAddress);
            restClient.HttpClient.DefaultRequestHeaders.Accept.Clear();
            restClient.HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Then(@"the response message should contain (.*)")]
        public void ThenTheResponseMessageShouldContain(string p0)
        {
            restClient.Result.Should().NotBeNull();
            restClient.Result.Should().Contain(p0);
        }

        [Then(@"the response http status should be (.*)")]
        public void ThenTheResponseHttpStatusShouldBe(string httpStatusCode)
        {
            restClient.HttpResponseMessage.ReasonPhrase.Should().Be(httpStatusCode);
        }
    }
}

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    using FluentAssertions;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
    using TechTalk.SpecFlow;

    [Binding]
    public class ApiStepDefinitionBase
    {
        private readonly RestClientResult _restClientResult;

        public ApiStepDefinitionBase(RestClientResult restClientResult)
        {
            _restClientResult = restClientResult;
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
           
        }

        [Then(@"the response message should contain (.*)")]
        public void ThenTheResponseMessageShouldContain(string p0)
        {
            _restClientResult.JsonResult.Should().NotBeNull();
            _restClientResult.JsonResult.Should().Contain(p0);
        }

        [Then(@"the response http status should be (.*)")]
        public void ThenTheResponseHttpStatusShouldBe(string httpStatusCode)
        {
            _restClientResult.HttpResponseMessage.ReasonPhrase.Should().Be(httpStatusCode);
        }

        [Then(@"the Location Header should be set")]
        public void ThenTheLocationHeaderShouldBeSet()
        {
            _restClientResult.HttpResponseMessage.Headers.Location.Should().NotBeNull();
        }
    }
}

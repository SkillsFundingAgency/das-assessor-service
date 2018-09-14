using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services
{
    public class OrganisationQueryService : BaseRestServce
    {
        private readonly RestClientResult _restClientResult;

        public OrganisationQueryService(RestClientResult restClientResult,
            IWebConfiguration webConfiguration) : base(webConfiguration)
        {
            _restClientResult = restClientResult;
        }

        public RestClientResult GetOrganisations()
        {
            var response = HttpClient.GetAsync(
                "api/v1/organisations").Result;

            _restClientResult.JsonResult = response.Content.ReadAsStringAsync().Result;
            _restClientResult.HttpResponseMessage = response;

            return _restClientResult;
        }

        public RestClientResult SearchOrganisationByUkPrn(int ukprn)
        {
            _restClientResult.HttpResponseMessage = HttpClient.GetAsync(
                $"api/v1/organisations/{ukprn}").Result;
            _restClientResult.JsonResult = _restClientResult.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            return _restClientResult;
        }

    }
}

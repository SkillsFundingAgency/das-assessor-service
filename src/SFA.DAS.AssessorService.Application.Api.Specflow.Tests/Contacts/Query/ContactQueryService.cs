using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query
{
    public class ContactQueryService : BaseRestServce
    {
        private readonly RestClientResult _restClientResult;

        public ContactQueryService(RestClientResult restClientResult,
            IWebConfiguration webConfiguration) : base(webConfiguration)
        {
            _restClientResult = restClientResult;
        }

        public RestClientResult SearchForContactByUserName(string userName)
        {
            _restClientResult.HttpResponseMessage = HttpClient.GetAsync(
                $"api/v1/contacts/user/{userName}").Result;
            _restClientResult.JsonResult = _restClientResult.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            return _restClientResult;
        }

        public RestClientResult SearchForContactByOrganisationId(string endPointAssessorOrganisationId)
        {
            _restClientResult.HttpResponseMessage = HttpClient.GetAsync(
                $"api/v1/contacts/{endPointAssessorOrganisationId}").Result;

            _restClientResult.JsonResult = _restClientResult.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            return _restClientResult;
        }
    }
}

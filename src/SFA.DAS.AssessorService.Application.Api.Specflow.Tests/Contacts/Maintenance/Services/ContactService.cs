using System;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance.Services
{
    public class ContactService : BaseRestServce
    {
        private readonly RestClientResult _restClientResult;

        public ContactService(RestClientResult restClientResult,
            IWebConfiguration webConfiguration) : base(webConfiguration)
        {
            _restClientResult = restClientResult;
        }

        public RestClientResult PostContact(CreateContactRequest organisation)
        {
            var responseMessage = HttpClient.PostAsJsonAsync(
                "api/v1/contacts", organisation).Result;
            var jsonResult = responseMessage.Content.ReadAsStringAsync().Result;

            _restClientResult.HttpResponseMessage = responseMessage;
            _restClientResult.JsonResult = jsonResult;

            return _restClientResult;
        }

        public RestClientResult PutContact(UpdateContactRequest organisation)
        {
            var responseMessage = HttpClient.PutAsJsonAsync(
                "api/v1/contacts", organisation).Result;
            var jsonResult = responseMessage.Content.ReadAsStringAsync().Result;

            _restClientResult.HttpResponseMessage = responseMessage;
            _restClientResult.JsonResult = jsonResult;

            return _restClientResult;
        }

        public RestClientResult DeleteContact(string userName)
        {
            _restClientResult.HttpResponseMessage = HttpClient.DeleteAsJsonAsync($"api/v1/contacts?userName={userName}").Result;
            _restClientResult.JsonResult = String.Empty;

            return _restClientResult;
        }
    }
}


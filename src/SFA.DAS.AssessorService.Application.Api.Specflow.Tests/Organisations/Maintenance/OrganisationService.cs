using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    public class OrganisationService: BaseRestServce
    {
        private readonly RestClientResult _restClientResult;
       
        public OrganisationService(RestClientResult restClientResult)
        {
            _restClientResult = restClientResult;
        }

        public RestClientResult PostOrganisation(CreateOrganisationRequest organisation)
        {
            var responseMessage = HttpClient.PostAsJsonAsync(
                "api/v1/organisations", organisation).Result;
            var jsonResult = responseMessage.Content.ReadAsStringAsync().Result;

            _restClientResult.HttpResponseMessage = responseMessage;
            _restClientResult.JsonResult = jsonResult;

            return _restClientResult;
        }
    }
}

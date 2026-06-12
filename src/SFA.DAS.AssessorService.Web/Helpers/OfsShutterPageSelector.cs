using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.Helpers
{
    public class OfsShutterPageSelector
    {
        private readonly IOrganisationsApiClient _organisationsApiClient;

        public OfsShutterPageSelector(IOrganisationsApiClient organisationsApiClient)
        {
            _organisationsApiClient = organisationsApiClient;
        }

        public async Task<OfsShutterPageSelection> GetFromOrganisationAndStandard(EpaOrganisation organisation, AppliedStandardVersion standard)
        {
            bool isOfs = await _organisationsApiClient.IsOfsOrganisation(organisation);
            bool showNeedToRegisterPage = !isOfs;
            bool showNeedToSubmitIlrPage = isOfs && !standard.EpaoMustBeApprovedByRegulatorBody;
            return new OfsShutterPageSelection(showNeedToRegisterPage, showNeedToSubmitIlrPage);
        }
    }
}

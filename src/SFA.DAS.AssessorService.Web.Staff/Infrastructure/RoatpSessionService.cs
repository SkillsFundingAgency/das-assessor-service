namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using Newtonsoft.Json;
    using ViewModels.Roatp;

    public class RoatpSessionService : IRoatpSessionService
    {
        private ISessionService _sessionService;

        private const string _addOrganisationSessionKey = "Roatp_AddOrganisation";

        public RoatpSessionService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public AddOrganisationViewModel GetAddOrganisationDetails()
        {
            var modelJson = _sessionService.Get(_addOrganisationSessionKey);

            if (modelJson == null)
            {
                return null;
            }
            var model = JsonConvert.DeserializeObject<AddOrganisationViewModel>(modelJson);

            return model;
        }

        public void SetAddOrganisationDetails(AddOrganisationViewModel model)
        {
            var modelJson = JsonConvert.SerializeObject(model);

            _sessionService.Set(_addOrganisationSessionKey, modelJson);
        }

        public void ClearAddOrganisationDetails()
        {
            _sessionService.Remove(_addOrganisationSessionKey);
        }
    }
}

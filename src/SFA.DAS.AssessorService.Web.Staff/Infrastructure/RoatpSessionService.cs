namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using Newtonsoft.Json;
    using System;
    using ViewModels.Roatp;

    public class RoatpSessionService : IRoatpSessionService
    {
        private ISessionService _sessionService;

        public RoatpSessionService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public AddOrganisationViewModel GetAddOrganisationDetails(Guid id)
        {
            var sessionKey = $"Roatp_AddOrganisation_{id}";
            var modelJson = _sessionService.Get(sessionKey);

            if (modelJson == null)
            {
                return new AddOrganisationViewModel {OrganisationId = id};
            }
            var model = JsonConvert.DeserializeObject<AddOrganisationViewModel>(modelJson);

            return model;
        }

        public void SetAddOrganisationDetails(AddOrganisationViewModel model)
        {
            var sessionKey = $"Roatp_AddOrganisation_{model.OrganisationId}";
            
            var modelJson = JsonConvert.SerializeObject(model);

            _sessionService.Set(sessionKey, modelJson);
        }
    }
}

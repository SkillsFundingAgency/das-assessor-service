namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using System;
    using ViewModels.Roatp;

    public interface IRoatpSessionService
    {
        AddOrganisationViewModel GetAddOrganisationDetails(Guid id);
        void SetAddOrganisationDetails(AddOrganisationViewModel model);
    }
}

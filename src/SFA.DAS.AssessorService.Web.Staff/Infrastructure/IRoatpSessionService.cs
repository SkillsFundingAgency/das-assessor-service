namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using ViewModels.Roatp;

    public interface IRoatpSessionService
    {
        AddOrganisationViewModel GetAddOrganisationDetails();
        void SetAddOrganisationDetails(AddOrganisationViewModel model);
        void ClearAddOrganisationDetails();
    }
}

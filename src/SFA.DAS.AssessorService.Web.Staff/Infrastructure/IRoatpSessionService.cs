namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using ViewModels.Roatp;

    public interface IRoatpSessionService
    {
        AddOrganisationViewModel GetAddOrganisationDetails();
        void SetAddOrganisationDetails(AddOrganisationViewModel model);
        void ClearAddOrganisationDetails();
        OrganisationSearchResultsViewModel GetSearchResults();
        void SetSearchResults(OrganisationSearchResultsViewModel model);
        void ClearSearchResults();
        string GetSearchTerm();
        void SetSearchTerm(string searchTerm);
        void ClearSearchTerm();
    }
}

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class StaffSearchRequest
    {
        public StaffSearchRequest(string searchQuery, int page)
        {
            SearchQuery = searchQuery;
            Page = page;
        }

        public string SearchQuery { get; set; }
        public int Page { get; }
    }
}

namespace SFA.DAS.AssessorService.ExternalApis.Ilr.Types
{
    public class SearchRequest
    {
        public SearchType SearchType { get; set; }
        public string Uln { get; set; }
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
    }
}
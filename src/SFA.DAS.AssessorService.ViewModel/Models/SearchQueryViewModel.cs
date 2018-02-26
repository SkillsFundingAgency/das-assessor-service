namespace SFA.DAS.AssessorService.ViewModel.Models
{
    public class SearchQueryViewModel
    {
        public string SearchType { get; set; }
        public string Uln { get; set; }
        public string Surname { get; set; }
        public int DobDay { get; set; }
        public int DobMonth { get; set; }
        public int DobYear { get; set; }

        public string DateOfBirth => $"{DobDay:00}/{DobMonth:00}/{DobYear:0000}";
    }

    public static class SearchTypes
    {
        public static string Uln => "uln";
        public static string DobSurname => "dobsurname";
    }
}
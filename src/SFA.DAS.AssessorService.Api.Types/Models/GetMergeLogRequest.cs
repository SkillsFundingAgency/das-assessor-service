namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetMergeLogRequest
    {
        public int? PageSize { get; set; } = 10;
        public int? PageIndex { get; set; } = 1;
        public string SortColumn { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
        public string PrimaryEPAOId { get; set; }
        public string SecondaryEPAOId { get; set; }
    }
}

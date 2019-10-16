namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardNonApprovedCollation
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Title { get; set; }
        public StandardNonApprovedData StandardData { get; set; }
    }
}

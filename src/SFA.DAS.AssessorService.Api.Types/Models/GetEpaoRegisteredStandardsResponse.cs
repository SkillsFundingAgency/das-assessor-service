namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaoRegisteredStandardsResponse
    {
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public int Level { get; set; }
        public string ReferenceNumber { get; set; }
    }
}

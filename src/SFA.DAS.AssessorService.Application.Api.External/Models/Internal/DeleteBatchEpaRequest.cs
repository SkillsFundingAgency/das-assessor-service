namespace SFA.DAS.AssessorService.Application.Api.External.Models.Internal
{
    public class DeleteBatchEpaRequest
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public string Standard { get; set; }
        public string EpaReference { get; set; }

        public int UkPrn { get; set; }
    }
}

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Internal
{
    public class GetBatchCertificateRequest
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public string Standard { get; set; }

        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}

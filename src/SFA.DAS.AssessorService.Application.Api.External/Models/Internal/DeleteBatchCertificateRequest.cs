namespace SFA.DAS.AssessorService.Application.Api.External.Models.Internal
{
    public class DeleteBatchCertificateRequest
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public string Standard { get; set; }
        public string CertificateReference { get; set; }

        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}

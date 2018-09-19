namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class SubmitBatchCertificateRequest
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }
        public string CertificateReference { get; set; }

        public int UkPrn { get; set; }
        public string Username { get; set; }
    }
}

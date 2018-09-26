namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class DeleteCertificateRequest
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public int StandardCode { get; set; }
        public string CertificateReference { get; set; }

        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}

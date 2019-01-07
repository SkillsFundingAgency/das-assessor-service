namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class ApprovalResult
    {
        public string CertificateReference { get; set; }
        public string IsApproved { get; set; }
        public string PrivatelyFundedStatus { get; set; }
        public string ReasonForChange { get; set; }
    }
}
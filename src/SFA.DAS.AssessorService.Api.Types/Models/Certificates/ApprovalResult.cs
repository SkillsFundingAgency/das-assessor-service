namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class ApprovalResult
    {
        public string CertificateReference { get; set; }
        public string ReasonForRejection { get; set; }
        public string ApprovedStatus { get; set; }
    }
}
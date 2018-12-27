namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificatePostApprovalViewModel
    {
        public string UserName { get; set; }
        public string ActionHint { get; set; }
        public ApprovalResult[] ApprovalResults { get; set; }
    }
}
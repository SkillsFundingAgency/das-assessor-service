namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateApprovalViewModel
    {
        public string CertificateReference { get; set; }
        public string StandardCode { get; set; }
        public string Uln { get; set; }        
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
    }
}
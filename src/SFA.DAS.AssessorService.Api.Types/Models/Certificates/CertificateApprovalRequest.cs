using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateApprovalRequest : IRequest
    {
        public string UserName { get; set; }
        public ApprovalResult[] ApprovalResults { get; set; }    
    }
}
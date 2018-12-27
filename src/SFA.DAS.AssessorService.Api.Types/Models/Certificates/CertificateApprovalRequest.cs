using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateApprovalRequest : IRequest
    {
        public string userName { get; set; }
        public ApprovalResult[] ApprovalResults { get; set; }   
        public string ActionHint { get; set; }
    }
}
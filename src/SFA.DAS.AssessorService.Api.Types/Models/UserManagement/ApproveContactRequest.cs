using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class ApproveContactRequest : IRequest
    {
        public Guid ContactId { get; set; }
    }
    
    public class RejectContactRequest : IRequest
    {
        public Guid ContactId { get; set; }
    }
}
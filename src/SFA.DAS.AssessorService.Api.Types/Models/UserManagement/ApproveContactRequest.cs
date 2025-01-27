using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class ApproveContactRequest : IRequest<Unit>
    {
        public Guid ContactId { get; set; }
    }
    
    public class RejectContactRequest : IRequest<Unit>
    {
        public Guid ContactId { get; set; }
    }
}
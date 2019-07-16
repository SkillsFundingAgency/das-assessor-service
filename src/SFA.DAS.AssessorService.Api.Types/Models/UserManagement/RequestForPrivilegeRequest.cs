using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class RequestForPrivilegeRequest : IRequest
    {
        public Guid ContactId { get; set; }
        public Guid PrivilegeId { get; set; }
    }
}
using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class SetContactPrivilegesRequest : IRequest<SetContactPrivilegesResponse>
    {
        public Guid ContactId { get; set; }

        public Guid[] PrivilegeIds { get; set; }
    }
}
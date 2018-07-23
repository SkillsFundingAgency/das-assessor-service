using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class StaffUIReprintRequest : IRequest<StaffUIReprintResponse>
    {
        public StaffUIReprintRequest(Guid id, string userName)
        {
            Id = id;
            Username = userName;
        }

        public Guid Id { get; }
        public string Username { get;  }
    }
}
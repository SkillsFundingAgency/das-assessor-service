using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class StaffUIReprintRequest : IRequest<StaffUIReprintResponse>
    {        
        public Guid Id { get; set;  }
        public string Username { get; set; }
    }
}
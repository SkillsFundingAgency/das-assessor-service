using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateContactStatusRequest : IRequest
    {
        public UpdateContactStatusRequest(Guid id, string status)
        {
            Id = id;
            Status = status;
        }
        public Guid Id { get;  }
        public string Status { get; }
    }
}

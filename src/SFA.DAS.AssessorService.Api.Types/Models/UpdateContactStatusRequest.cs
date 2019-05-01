using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateContactStatusRequest : IRequest
    {
        public UpdateContactStatusRequest(string id, string status)
        {
            Id = id;
            Status = status;
        }
        public string Id { get;  }
        public string Status { get; }
    }
}

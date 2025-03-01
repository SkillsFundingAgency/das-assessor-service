﻿using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateContactStatusRequest : IRequest<Unit>
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

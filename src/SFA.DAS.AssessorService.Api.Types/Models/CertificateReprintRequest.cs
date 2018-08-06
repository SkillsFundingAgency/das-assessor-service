﻿using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CertificateReprintRequest : IRequest<CertificateReprintResponse>
    {        
        public Guid Id { get; set;  }
        public string Username { get; set; }
    }
}
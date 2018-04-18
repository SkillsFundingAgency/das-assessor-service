﻿using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificatesRequest : IRequest<List<CertificateResponse>>
    {
        public string Status { get; set; }
    }
}
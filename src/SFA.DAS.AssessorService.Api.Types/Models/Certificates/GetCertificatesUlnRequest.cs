using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificatesUlnRequest : IRequest<List<ApprenticeCertificateSummary>>
    {
        public long Uln { get; set; }
    }
}
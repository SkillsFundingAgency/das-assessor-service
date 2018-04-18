using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    static class CertificateResponseExtensions
    {
        public static List<CertificateResponse> MapToCertificateResponses(this List<Certificate> certificates)
        {
            var results = Mapper.Map<List<CertificateResponse>>(certificates);

            return results;
        }
    }
}
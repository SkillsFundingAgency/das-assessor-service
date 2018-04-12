using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesHandler : IRequestHandler<GetCertificates, List<CertificateResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificatesHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<List<CertificateResponse>> Handle(GetCertificates request,
            CancellationToken cancellationToken)
        {
            var certificates = await _certificateRepository.GetCertificates(request.Status);
            var certificateResponses = Mapper.Map<List<Certificate>, List<CertificateResponse>>(certificates);
            return certificateResponses;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesHistoryHandler : IRequestHandler<GetCertificateHistoryRequest, List<CertificateHistoryResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificatesHistoryHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
       
        public async Task<List<CertificateHistoryResponse>> Handle(GetCertificateHistoryRequest request, CancellationToken cancellationToken)
        {
            var certificates = await _certificateRepository.GetCertificateHistory();

            // Please Note:- Cannot seem to automap this with custom value/type converters
            // so dealing with it manually for now.
            var certificateHistoryResponses = MapCertificates(certificates);
            return certificateHistoryResponses;
        }

        private List<CertificateHistoryResponse> MapCertificates(List<Certificate> certificates)
        {
            var certificateResponses =  certificates.Select(
                q => new CertificateHistoryResponse
                {

                }
            );

            return certificateResponses.ToList();
        }
    }
}
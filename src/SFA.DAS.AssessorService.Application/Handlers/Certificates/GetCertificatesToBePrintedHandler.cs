using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesToBePrintedHandler : IRequestHandler<GetCertificatesToBePrintedRequest, List<Certificate>>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificatesToBePrintedHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
        public async Task<List<Certificate>> Handle(GetCertificatesToBePrintedRequest request, CancellationToken cancellationToken)
        {
            return await _certificateRepository.GetCertificatesToBePrinted();
        }
    }
}
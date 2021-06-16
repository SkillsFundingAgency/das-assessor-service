using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetContactPreviousAddressesHandler : IRequestHandler<GetContactPreviousAddressesRequest, CertificateAddress>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetContactPreviousAddressesHandler(
            ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<CertificateAddress> Handle(GetContactPreviousAddressesRequest request, CancellationToken cancellationToken)
        {
            var certificateAddress = await _certificateRepository.GetContactPreviousAddress(request.Username);
            return certificateAddress;
        }
    }
}



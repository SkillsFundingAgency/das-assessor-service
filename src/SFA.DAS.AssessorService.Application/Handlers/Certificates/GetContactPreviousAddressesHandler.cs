using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Handlers.Certificates
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
            var certificateAddress = await _certificateRepository.GetContactPreviousAddress(request.EpaOrgId, request.EmployerAccountId);
            return certificateAddress;
        }
    }
}



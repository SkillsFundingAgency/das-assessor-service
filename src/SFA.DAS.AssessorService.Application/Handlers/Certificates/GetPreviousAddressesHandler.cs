using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetPreviousAddressesHandler : IRequestHandler<GetPreviousAddressesRequest, List<CertificateAddress>>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetPreviousAddressesHandler(
            ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<List<CertificateAddress>> Handle(GetPreviousAddressesRequest request, CancellationToken cancellationToken)
        {
            var certificateAddresses = await _certificateRepository.GetPreviousAddresses(request.Username);
            return certificateAddresses.ToList()
                .OrderBy(q => q.AddressLine1)
                .ThenBy(q => q.AddressLine2)
                .ThenBy(q => q.AddressLine3)
                .ThenBy(q => q.City)
                .ThenBy(q => q.PostCode)
                .ToList();
        }
    }
}



using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetAddressesHandler : IRequestHandler<GetAddressesRequest, List<CertificateAddressResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<StartCertificateHandler> _logger;

        public GetAddressesHandler(
            IContactQueryRepository contactQueryRepository,
            ICertificateRepository certificateRepository, IIlrRepository ilrRepository,            
            IOrganisationQueryRepository organisationQueryRepository, ILogger<StartCertificateHandler> logger)
        {
            _contactQueryRepository = contactQueryRepository;
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;        
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
        }
     
        public async Task<List<CertificateAddressResponse>> Handle(GetAddressesRequest request, CancellationToken cancellationToken)
        {
            var contact = await _contactQueryRepository.GetContact(request.Username);
            var organisation = await _organisationQueryRepository.Get(contact.EndPointAssessorOrganisationId);

            var certificateAddresses = await _certificateRepository.GetCertificateAddresses(organisation.Id);
            var addresses = Mapper.Map<List<CertificateAddressResponse>>(certificateAddresses);

            return addresses;
        }
    }
}



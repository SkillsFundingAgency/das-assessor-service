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
    public class GetCertificateAddressesHandler : IRequestHandler<GetAddressesRequest, List<CertificateAddressResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ICertificateRepository _certificateRepository;        
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<StartCertificateHandler> _logger;

        public GetCertificateAddressesHandler(
            IContactQueryRepository contactQueryRepository,
            ICertificateRepository certificateRepository,           
            IOrganisationQueryRepository organisationQueryRepository)         
        {
            _contactQueryRepository = contactQueryRepository;
            _certificateRepository = certificateRepository;                
            _organisationQueryRepository = organisationQueryRepository;            
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



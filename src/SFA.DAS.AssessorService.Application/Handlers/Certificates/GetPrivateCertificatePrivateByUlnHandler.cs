using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetPrivateCertificatePrivateByUlnHandler : IRequestHandler<GetPrivateCertificatePrivateByUlnRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetPrivateCertificatePrivateByUlnHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Certificate> Handle(GetPrivateCertificatePrivateByUlnRequest request,
            CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetPrivateCertificate(request.Uln, request.EndPointAssessorOrganisationId, request.LastName);
            return certificate;
        }
    }
}
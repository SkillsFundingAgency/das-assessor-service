using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffUIReprintHandler : IRequestHandler<StaffUIReprintRequest, StaffUIReprintResponse>
    {
        private readonly ICertificateRepository _certificateRepository;

        public StaffUIReprintHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<StaffUIReprintResponse> Handle(StaffUIReprintRequest request,
            CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.Id);
            if (certificate == null)
                throw new NotFound();          

            if (certificate.Status == Domain.Consts.CertificateStatus.Printed
                || certificate.Status == Domain.Consts.CertificateStatus.Reprint)
            {
                certificate.Status = Domain.Consts.CertificateStatus.Reprint;
                await _certificateRepository.Update(certificate, request.Username,
                    action: SFA.DAS.AssessorService.Domain.Consts.CertificateActions.Reprint);

                var staffUiReprintResponse = new StaffUIReprintResponse
                {
                    Certificate = certificate
                };
             
                return staffUiReprintResponse;
            }
            else
            {
                throw new NotFound();
            }
        }
    }
}
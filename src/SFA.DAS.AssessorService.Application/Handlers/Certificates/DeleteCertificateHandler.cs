using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class DeleteCertificateHandler : IRequestHandler<DeleteCertificateRequest, Unit>
    {
        private readonly ICertificateRepository _certificateRepository;        

        public DeleteCertificateHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Unit> Handle(DeleteCertificateRequest request, CancellationToken cancellationToken)
        {
            await _certificateRepository.Delete(request.Uln, request.StandardCode, request.UserName, CertificateActions.Delete, true, request.ReasonForChange, request.IncidentNumber);
            return Unit.Value;
        }
    }
}

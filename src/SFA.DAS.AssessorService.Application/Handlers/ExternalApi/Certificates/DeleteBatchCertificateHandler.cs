using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates
{
    public class DeleteBatchCertificateHandler : IRequestHandler<DeleteBatchCertificateRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<DeleteBatchCertificateHandler> _logger;

        public DeleteBatchCertificateHandler(ICertificateRepository certificateRepository, ILogger<DeleteBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            await DeleteCertificate(request);
            return Unit.Value;
        }

        private async Task DeleteCertificate(DeleteBatchCertificateRequest request)
        {
            _logger.LogInformation("DeleteCertificate Before set Certificate to Deleted in db");
            await _certificateRepository.Delete(request.Uln, request.StandardCode, ExternalApiConstants.ApiUserName, CertificateActions.Delete);
            _logger.LogInformation("DeleteCertificate Certificate set to Deleted in db");
        }
    }
}

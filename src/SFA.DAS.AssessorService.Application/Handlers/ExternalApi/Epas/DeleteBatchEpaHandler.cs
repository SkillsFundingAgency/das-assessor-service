using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas
{
    public class DeleteBatchEpaHandler : IRequestHandler<DeleteBatchEpaRequest, Unit>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<DeleteBatchEpaHandler> _logger;

        public DeleteBatchEpaHandler(ICertificateRepository certificateRepository, ILogger<DeleteBatchEpaHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteBatchEpaRequest request, CancellationToken cancellationToken)
        {
            await DeleteEpaDetails(request);
            return Unit.Value;
        }

        private async Task DeleteEpaDetails(DeleteBatchEpaRequest request)
        {
            _logger.LogInformation("DeleteEpaDetails Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate is null)
            {
                throw new NotFoundException();
            }
            else
            {
                _logger.LogInformation("DeleteEpaDetails Before Removing EpaDetails CertificateData");
                certificate.CertificateData.EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() };
                await _certificateRepository.UpdateStandardCertificate(certificate, ExternalApiConstants.ApiUserName, CertificateActions.Epa);
            }

            _logger.LogInformation("DeleteEpaDetails Before set Certificate to Deleted in db");
            await _certificateRepository.Delete(request.Uln, request.StandardCode, ExternalApiConstants.ApiUserName, CertificateActions.Delete);
            _logger.LogInformation("DeleteEpaDetails Certificate set to Deleted in db");
        }
    }
}

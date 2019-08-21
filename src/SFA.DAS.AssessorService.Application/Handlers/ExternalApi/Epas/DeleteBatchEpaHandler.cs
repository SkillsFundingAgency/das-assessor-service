using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas
{
    public class DeleteBatchEpaHandler : IRequestHandler<DeleteBatchEpaRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<DeleteBatchEpaHandler> _logger;

        public DeleteBatchEpaHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<DeleteBatchEpaHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task Handle(DeleteBatchEpaRequest request, CancellationToken cancellationToken)
        {
            await DeleteEpaDetails(request);
        }

        private async Task DeleteEpaDetails(DeleteBatchEpaRequest request)
        {
            _logger.LogInformation("DeleteEpaDetails Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate is null)
            {
                throw new NotFound();
            }
            else
            {
                _logger.LogInformation("DeleteEpaDetails Before Removing EpaDetails CertificateData");
                var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                certData.EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() };
                certificate.CertificateData = JsonConvert.SerializeObject(certData);
                await _certificateRepository.Update(certificate, ExternalApiConstants.ApiUserName, CertificateActions.Epa);
            }

            _logger.LogInformation("DeleteEpaDetails Before set Certificate to Deleted in db");
            await _certificateRepository.Delete(request.Uln, request.StandardCode, ExternalApiConstants.ApiUserName, CertificateActions.Delete);
            _logger.LogInformation("DeleteEpaDetails Certificate set to Deleted in db");
        }
    }
}

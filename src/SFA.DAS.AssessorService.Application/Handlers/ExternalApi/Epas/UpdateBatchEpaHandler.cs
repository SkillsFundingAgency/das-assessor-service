using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas
{
    public class UpdateBatchEpaHandler : IRequestHandler<UpdateBatchEpaRequest, EpaDetails>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateBatchEpaHandler> _logger;

        public UpdateBatchEpaHandler(ICertificateRepository certificateRepository, ILogger<UpdateBatchEpaHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<EpaDetails> Handle(UpdateBatchEpaRequest request, CancellationToken cancellationToken)
        {
            return await UpdateEpaDetails(request);
        }

        private async Task<EpaDetails> UpdateEpaDetails(UpdateBatchEpaRequest request)
        {
            _logger.LogInformation("UpdateEpaDetails Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate is null)
            {
                throw new NotFound();
            }
            else
            {
                certificate = EpaHelpers.ResetCertificateData(certificate);
            }

            _logger.LogInformation("UpdateEpaDetails Before Combining EpaDetails");
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            certData.EpaDetails = new EpaDetails { EpaReference = certificate.CertificateReference, Epas = new List<EpaRecord>() };

            // Always Update Version & Option
            // It was either retrieved from the old epa record if set, or overwriting the previous auto select
            // This could wipe out option if it was previously set and not supplied but that is a valid scenario.
            certData.Version = request.Version;
            certData.CourseOption = request.CourseOption;

            if (request.EpaDetails?.Epas != null)
            {
                foreach (var epa in request.EpaDetails.Epas)
                {
                    epa.EpaOutcome = EpaHelpers.NormalizeEpaOutcome(epa.EpaOutcome);
                    certData.EpaDetails.Epas.Add(epa);
                }
            }

            var latestEpaRecord = certData.EpaDetails.Epas.OrderByDescending(epa => epa.EpaDate).FirstOrDefault();
            certData.EpaDetails.LatestEpaDate = latestEpaRecord?.EpaDate;
            certData.EpaDetails.LatestEpaOutcome = latestEpaRecord?.EpaOutcome;

            var epaAction = CertificateActions.Epa;
            if (latestEpaRecord?.EpaOutcome.Equals(EpaOutcome.Fail, System.StringComparison.InvariantCultureIgnoreCase) == true)
            {
                certData.AchievementDate = latestEpaRecord?.EpaDate;
                certData.OverallGrade = CertificateGrade.Fail;
                certificate.Status = CertificateStatus.Submitted;
                epaAction = CertificateActions.Submit;
            }

            _logger.LogInformation("UpdateEpaDetails Before Update CertificateData");
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            _logger.LogInformation("UpdateEpaDetails Before Update Cert in db");
            await _certificateRepository.Update(certificate, ExternalApiConstants.ApiUserName, epaAction);

            return certData.EpaDetails;
        }
    }
}

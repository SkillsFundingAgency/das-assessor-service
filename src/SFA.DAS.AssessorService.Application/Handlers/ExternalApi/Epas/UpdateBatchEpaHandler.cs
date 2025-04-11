using System.Collections.Generic;
using System.Linq;
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
                throw new NotFoundException();
            }
            else
            {
                certificate = EpaHelpers.ResetCertificateData(certificate);
            }

            _logger.LogInformation("UpdateEpaDetails Before Combining EpaDetails");
            certificate.CertificateData.EpaDetails = new EpaDetails { EpaReference = certificate.CertificateReference, Epas = new List<EpaRecord>() };
            certificate.CertificateData.Version = string.IsNullOrWhiteSpace(request.Version) ? null : request.Version;
            certificate.CertificateData.CourseOption = string.IsNullOrWhiteSpace(request.CourseOption) ? null : request.CourseOption;

            if (request.EpaDetails?.Epas != null)
            {
                foreach (var epa in request.EpaDetails.Epas)
                {
                    epa.EpaOutcome = EpaHelpers.NormalizeEpaOutcome(epa.EpaOutcome);
                    certificate.CertificateData.EpaDetails.Epas.Add(epa);
                }
            }

            var latestEpaRecord = certificate.CertificateData.EpaDetails.Epas.OrderByDescending(epa => epa.EpaDate).FirstOrDefault();
            certificate.CertificateData.EpaDetails.LatestEpaDate = latestEpaRecord?.EpaDate;
            certificate.CertificateData.EpaDetails.LatestEpaOutcome = latestEpaRecord?.EpaOutcome;

            var epaAction = CertificateActions.Epa;
            if (latestEpaRecord?.EpaOutcome.Equals(EpaOutcome.Fail, System.StringComparison.InvariantCultureIgnoreCase) == true)
            {
                certificate.CertificateData.AchievementDate = latestEpaRecord?.EpaDate;
                certificate.CertificateData.OverallGrade = CertificateGrade.Fail;
                certificate.Status = CertificateStatus.Submitted;
                epaAction = CertificateActions.Submit;
            }
            else
            {
                certificate.CertificateData.AchievementDate = null;
                certificate.CertificateData.OverallGrade = null;
                certificate.Status = CertificateStatus.Draft;
            }

            _logger.LogInformation("UpdateEpaDetails Before Update Cert in db");
            await _certificateRepository.UpdateStandardCertificate(certificate, ExternalApiConstants.ApiUserName, epaAction);

            return certificate.CertificateData.EpaDetails;
        }
    }
}

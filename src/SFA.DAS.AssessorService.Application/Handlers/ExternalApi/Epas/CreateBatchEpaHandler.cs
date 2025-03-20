using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas
{
    public class CreateBatchEpaHandler : IRequestHandler<CreateBatchEpaRequest, EpaDetails>
    {
        private readonly IMediator _mediator;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<CreateBatchEpaHandler> _logger;

        public CreateBatchEpaHandler(ICertificateRepository certificateRepository, ILogger<CreateBatchEpaHandler> logger, IMediator mediator)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<EpaDetails> Handle(CreateBatchEpaRequest request, CancellationToken cancellationToken)
        {
            return await CreateNewEpa(request);
        }

        private async Task<EpaDetails> CreateNewEpa(CreateBatchEpaRequest request)
        {
            _logger.LogInformation("CreateNewEpa Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate is null)
            {
                _logger.LogInformation("CreateNewEpa Before StartCertificateRequest");
                var startCertificateRequest = new StartCertificateRequest { StandardCode = request.StandardCode, UkPrn = request.UkPrn, Uln = request.Uln, Username = ExternalApiConstants.ApiUserName, CourseOption = request.CourseOption, StandardUId = request.StandardUId };
                certificate = await _mediator.Send(startCertificateRequest);
            }
            else
            {
                certificate = EpaHelpers.ResetCertificateData(certificate);
            }

            if (certificate is null)
            {
                _logger.LogError($"CreateNewEpa StartCertificateRequest did not create Certificate for Uln {request.Uln} StandardCode {request.StandardCode}");
                throw new NotFoundException();
            }
            certificate.Status = Domain.Consts.CertificateStatus.Draft;

            _logger.LogInformation("CreateNewEpa Before Adding EPAs");
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
            if (latestEpaRecord?.EpaOutcome.Equals(EpaOutcome.Fail, StringComparison.InvariantCultureIgnoreCase) == true)
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

            _logger.LogInformation("CreateNewEpa Before Update Cert in db");
            
            await _certificateRepository.UpdateStandardCertificate(certificate, ExternalApiConstants.ApiUserName, epaAction);

            return certificate.CertificateData.EpaDetails;
        }
    }
}

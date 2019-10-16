using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateHandler : IRequestHandler<UpdateCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificateHandler> _logger;

        public UpdateCertificateHandler(ICertificateRepository certificateRepository, ILogger<UpdateCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(UpdateCertificateRequest request, CancellationToken cancellationToken)
        {
            if (request.Certificate.Status == Domain.Consts.CertificateStatus.Submitted)
            {
                _logger.LogInformation(LoggingConstants.CertificateSubmitted);
                _logger.LogInformation($"Certificate with ID: {request.Certificate.Id} Submitted with reference of {request.Certificate.CertificateReference}");

                var certData = JsonConvert.DeserializeObject<CertificateData>(request.Certificate.CertificateData);
                if (certData != null)
                {
                    var epaDetails = certData.EpaDetails ??
                        new EpaDetails
                        {
                            Epas = new List<EpaRecord>()
                        };

                    var epaOutcome = certData.OverallGrade == CertificateGrade.Fail ? EpaOutcome.Fail : EpaOutcome.Pass;

                    if (certData.AchievementDate != null && 
                        !epaDetails.Epas.Any(rec => rec.EpaDate == certData.AchievementDate.Value && rec.EpaOutcome.Equals(epaOutcome, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        epaDetails.Epas.Add(new EpaRecord
                        {
                            EpaDate = certData.AchievementDate.Value,
                            EpaOutcome = epaOutcome
                        });

                        // sort pass outcomes before fail outcomes as pass is the final state even if earlier than the fail
                        var latestRecord = epaDetails.Epas
                            .OrderByDescending(epa => epa.EpaOutcome != EpaOutcome.Fail ? 1 : 0)
                            .ThenByDescending(epa => epa.EpaDate)
                            .First();

                        epaDetails.LatestEpaDate = latestRecord.EpaDate;
                        epaDetails.LatestEpaOutcome = latestRecord.EpaOutcome;
                        epaDetails.EpaReference = request.Certificate.CertificateReference;
                    }

                    certData.EpaDetails = epaDetails;
                    request.Certificate.CertificateData = JsonConvert.SerializeObject(certData);
                }
            }

            // NOTE: Unlike UpdateBatchCertificateHandler (External API) the status is altered from the UI in all cases

            var logs = await _certificateRepository.GetCertificateLogsFor(request.Certificate.Id);
            var latestLogEntry = logs.OrderByDescending(l => l.EventTime).FirstOrDefault();
            if (latestLogEntry != null && latestLogEntry.Action == request.Action && string.IsNullOrWhiteSpace(request.ReasonForChange))
            {
                return await _certificateRepository.Update(request.Certificate, request.Username, request.Action, updateLog:false);
            }

            return await _certificateRepository.Update(request.Certificate, request.Username, request.Action, updateLog: true, reasonForChange: request.ReasonForChange);
        }
    }
}
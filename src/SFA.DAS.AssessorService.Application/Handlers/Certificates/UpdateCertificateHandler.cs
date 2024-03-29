﻿using System;
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
        private readonly IStandardRepository _standardRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateCertificateHandler> _logger;

        public UpdateCertificateHandler(ICertificateRepository certificateRepository, IStandardRepository standardRepository, IMediator mediator, ILogger<UpdateCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _standardRepository = standardRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Certificate> Handle(UpdateCertificateRequest request, CancellationToken cancellationToken)
        {
            // NOTE: Unlike UpdateBatchCertificateHandler (External API) the status is altered from the UI in all cases

            var currentCertificate = await _mediator.Send(new GetCertificateRequest(request.Certificate.Id, false));
            if (currentCertificate == null)
            {
                throw new ArgumentException($"Certificate with ID: {request.Certificate.Id} was not found");
            }

            var currentData = JsonConvert.DeserializeObject<CertificateData>(currentCertificate.CertificateData);
            var updatedData = JsonConvert.DeserializeObject<CertificateData>(request.Certificate.CertificateData);

            if (request.Certificate.Status == CertificateStatus.Submitted ||
                request.Certificate.Status == CertificateStatus.ToBeApproved)
            {
                _logger.LogInformation(LoggingConstants.CertificateSubmitted);
                _logger.LogInformation($"Certificate with ID: {request.Certificate.Id} Submitted with reference of {request.Certificate.CertificateReference}");

                updatedData = HandleEpaDetailsUpdate(updatedData, request.Certificate.CertificateReference);
            }
            else
            {
                _logger.LogInformation($"Certificate with ID: {request.Certificate.Id} Updated with reference of {request.Certificate.CertificateReference}");
            }

            updatedData = CertificateDataSendToUpdater.HandleSendToUpdate(currentCertificate, currentData, updatedData);

            updatedData = await CertificateDataVersionChangeUpdater.UpdateCoronationEmblemAndStandardIfNeeded(currentData, updatedData, _standardRepository);

            request.Certificate.CertificateData = JsonConvert.SerializeObject(updatedData);

            return await Update(request);
        }

        private CertificateData HandleEpaDetailsUpdate(CertificateData updatedData, string certificateReference)
        {
            if (updatedData != null)
            {
                var epaDetails = updatedData.EpaDetails ??
                    new EpaDetails
                    {
                        Epas = new List<EpaRecord>()
                    };

                var epaOutcome = updatedData.OverallGrade == CertificateGrade.Fail ? EpaOutcome.Fail : EpaOutcome.Pass;

                if (updatedData.AchievementDate != null &&
                    !epaDetails.Epas.Any(rec => rec.EpaDate == updatedData.AchievementDate.Value && rec.EpaOutcome.Equals(epaOutcome, StringComparison.InvariantCultureIgnoreCase)))
                {
                    epaDetails.Epas.Add(new EpaRecord
                    {
                        EpaDate = updatedData.AchievementDate.Value,
                        EpaOutcome = epaOutcome
                    });

                    // sort pass outcomes before fail outcomes as pass is the final state even if earlier than the fail
                    var latestRecord = epaDetails.Epas
                        .OrderByDescending(epa => epa.EpaOutcome != EpaOutcome.Fail ? 1 : 0)
                        .ThenByDescending(epa => epa.EpaDate)
                        .First();

                    epaDetails.LatestEpaDate = latestRecord.EpaDate;
                    epaDetails.LatestEpaOutcome = latestRecord.EpaOutcome;
                    epaDetails.EpaReference = certificateReference;
                }

                updatedData.EpaDetails = epaDetails;
            }

            return updatedData;
        }

        private async Task<Certificate> Update(UpdateCertificateRequest request)
        {
            var logs = await _certificateRepository.GetCertificateLogsFor(request.Certificate.Id);
            var latestLogEntry = logs.OrderByDescending(l => l.EventTime).FirstOrDefault();

            if (latestLogEntry != null && latestLogEntry.Action == request.Action && latestLogEntry.CertificateData == request.Certificate.CertificateData && string.IsNullOrWhiteSpace(request.ReasonForChange))
            {
                return await _certificateRepository.Update(request.Certificate, request.Username, request.Action, updateLog: false);
            }

            return await _certificateRepository.Update(request.Certificate, request.Username, request.Action, updateLog: true, reasonForChange: request.ReasonForChange);
        }
    }
}

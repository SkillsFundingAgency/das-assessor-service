using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Helpers;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class PrintProcessFlowCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly ISanitiserService _sanitiserService;
        private readonly ICoverLetterService _coverLetterService;
        private readonly IIFACertificateService _ifaCertificateService;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly INotificationService _notificationService;

        public PrintProcessFlowCommand(IAggregateLogger aggregateLogger,
            ISanitiserService sanitiserService,
            ICoverLetterService coverLetterService,
            IIFACertificateService ifaCertificateService,
            IAssessorServiceApi assessorServiceApi,
            INotificationService notificationService)
        {
            _aggregateLogger = aggregateLogger;
            _sanitiserService = sanitiserService;
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _assessorServiceApi = assessorServiceApi;
            _notificationService = notificationService;
        }

        public async Task Execute()
        {
            try
            {
                _aggregateLogger.LogInfo("Function Started");
                _aggregateLogger.LogInfo("Print Function Flow Started");

                _aggregateLogger.LogInfo("Accessing Environment variables");
                _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

                var batchLogResponse = await _assessorServiceApi.GetCurrentBatchLog();

                if (await AnythingToProcess(batchLogResponse))
                {
                    var batchNumber = batchLogResponse.BatchNumber + 1;
                    var certificates = (await _assessorServiceApi.GetCertificatesToBePrinted()).ToList();

                    var sanitizedCertificateResponses = _sanitiserService.Sanitise(certificates);
                    if (sanitizedCertificateResponses.Count == 0)
                    {
                        _aggregateLogger.LogInfo("Nothing to Process");
                    }
                    else
                    {
                        var batchLogRequest = new CreateBatchLogRequest
                        {
                            BatchNumber = batchNumber,
                            FileUploadStartTime = DateTime.Now,
                            Period = DateTime.Now.ToString("MMyy"),
                            BatchCreated = DateTime.Now,
                            CertificatesFileName =
                                $"IFA-Certificate-{DateTime.Now:MMyy}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx"
                    };

                    var coverLettersProduced =
                        await _coverLetterService.Create(batchNumber, sanitizedCertificateResponses);
                    await _ifaCertificateService.Create(batchNumber, sanitizedCertificateResponses,
                        coverLettersProduced);

                    await _notificationService.Send(batchNumber, sanitizedCertificateResponses,
                        coverLettersProduced);

                    batchLogRequest.FileUploadEndTime = DateTime.Now;
                    batchLogRequest.NumberOfCertificates = coverLettersProduced.CoverLetterCertificates.Count;
                    batchLogRequest.NumberOfCoverLetters = coverLettersProduced.CoverLetterFileNames.Count;
                    batchLogRequest.ScheduledDate =
                        new ScheduledDates().GetThisScheduledDate(DateTime.Now, batchLogResponse.ScheduledDate);

                    await _assessorServiceApi.CreateBatchLog(batchLogRequest);

                    await _assessorServiceApi.ChangeStatusToPrinted(batchNumber, sanitizedCertificateResponses);
                }
            }
                else
                {
                _aggregateLogger.LogInfo("Nothing to Process");
            }
        }
            catch (Exception e)
            {
                _aggregateLogger.LogError("Function Errored", e);
                throw;
            }
}

private async Task<bool> AnythingToProcess(BatchLogResponse batchLogResponse)
{
    if ((await _assessorServiceApi.GetCertificatesToBePrinted()).Any())
    {
        var today = DateTime.Now;
        if (today >= batchLogResponse.ScheduledDate.AddDays(7))
            return true;
    }

    return false;
}

private DateTime GetNextScheduledDate(DateTime scheduledDateTime)
{
    var nextDate = new ScheduledDates().GetNextScheduledDate(DateTime.Now, scheduledDateTime);
    return nextDate;
}
    }
}

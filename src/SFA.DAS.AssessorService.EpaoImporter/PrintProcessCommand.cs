using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class PrintProcessCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IPrintingSpreadsheetCreator _printingSpreadsheetCreator;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly INotificationService _notificationService;
        private readonly IFileTransferClient _fileTransferClient;

        public PrintProcessCommand(IAggregateLogger aggregateLogger,
            IPrintingSpreadsheetCreator printingSpreadsheetCreator,
            IAssessorServiceApi assessorServiceApi,
            INotificationService notificationService,
            IFileTransferClient fileTransferClient)
        {
            _aggregateLogger = aggregateLogger;
            _printingSpreadsheetCreator = printingSpreadsheetCreator;
            _assessorServiceApi = assessorServiceApi;
            _notificationService = notificationService;
            _fileTransferClient = fileTransferClient;
        }

        public async Task Execute()
        {
            try
            {
                _aggregateLogger.LogInfo("Print Process Function Started");
                _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

                var scheduleRun = await _assessorServiceApi.GetSchedule(ScheduleType.PrintRun);
                if (scheduleRun == null)
                {
                    _aggregateLogger.LogInfo("Print Function not scheduled to run at this time.");
                    return;
                }

                var batchLogResponse = await _assessorServiceApi.GetCurrentBatchLog();

                var batchNumber = batchLogResponse.BatchNumber + 1;
                var certificates = (await _assessorServiceApi.GetCertificatesToBePrinted()).ToList()
                    .Sanitise(_aggregateLogger);

                if (certificates.Count == 0)
                {
                    _aggregateLogger.LogInfo("No certificates to process");
                }
                else
                {
                    var batchLogRequest = new CreateBatchLogRequest
                    {
                        BatchNumber = batchNumber,
                        FileUploadStartTime = DateTime.UtcNow,
                        Period = DateTime.UtcNow.UtcToTimeZoneTime().ToString("MMyy"),
                        BatchCreated = DateTime.UtcNow,
                        CertificatesFileName =
                            $"IFA-Certificate-{DateTime.UtcNow.UtcToTimeZoneTime():MMyy}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx"
                    };

                    _printingSpreadsheetCreator.Create(batchNumber, certificates);

                    await _notificationService.Send(batchNumber, certificates);

                    batchLogRequest.FileUploadEndTime = DateTime.UtcNow;
                    batchLogRequest.NumberOfCertificates = certificates.Count;
                    batchLogRequest.NumberOfCoverLetters = 0;
                    batchLogRequest.ScheduledDate = batchLogResponse.ScheduledDate;

                    await _fileTransferClient.LogUploadDirectory();

                    await _assessorServiceApi.CreateBatchLog(batchLogRequest);

                    await _assessorServiceApi.ChangeStatusToPrinted(batchNumber, certificates);
                }

                await _assessorServiceApi.CompleteSchedule(scheduleRun.Id);
            }
            catch (Exception e)
            {
                _aggregateLogger.LogError("Function Errored", e);
                throw;
            }
        }
    }
}

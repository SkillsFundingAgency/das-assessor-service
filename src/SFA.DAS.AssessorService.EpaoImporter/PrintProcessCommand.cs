using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
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
        private readonly IPrintingJsonCreator _printingJsonCreator;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly INotificationService _notificationService;
        private readonly IFileTransferClient _fileTransferClient;

        public PrintProcessCommand(IAggregateLogger aggregateLogger,
            IPrintingJsonCreator printingJsonCreator,
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
            _printingJsonCreator = printingJsonCreator;
        }

        public async Task Execute()
        {
            await UploadCertificateDetailsToPinter();
            await DownloadAndDeleteCertificatePrinterResponses();
        }

        private async Task DownloadAndDeleteCertificatePrinterResponses()
        {
            var fileList = await _fileTransferClient.GetListOfDownloadedFiles();

            // printResponse-MMYY-XXXXXX.json where XXX = 001, 002... 999999 etc
            const string pattern = @"^[Pp][Rr][Ii][Nn][Tt][Rr][Ee][Ss][Pp][Oo][Nn][Ss][Ee]-[0-9]{4}-[0-9]{1,6}.json";

            var certificateResponseFiles = fileList.Where(f => Regex.IsMatch(f, pattern));
            var filesToProcesses = certificateResponseFiles as string[] ?? certificateResponseFiles.ToArray();
            if (!filesToProcesses.Any())
            {
                _aggregateLogger.LogInfo("No certificate responses to process");
                return;
            }
           
             foreach (var fileToProcess in filesToProcesses)
             {
                 await ProcessEachFileToUploadThenDelete(fileToProcess);
             }
        }

        private async Task ProcessEachFileToUploadThenDelete(string fileToProcess)
        {
            var stringBatchResponse = await _fileTransferClient.DownloadFile(fileToProcess);
            var batchResponse = JsonConvert.DeserializeObject<BatchResponse>(stringBatchResponse);

            if (batchResponse?.Batch == null || batchResponse.Batch.BatchDate == DateTime.MinValue)
            {
                _aggregateLogger.LogInfo($"Could not process downloaded file to correct format [{fileToProcess}]");
                return;
            }

            

            batchResponse.Batch.DateOfResponse = DateTime.UtcNow;
            var batchNumber = batchResponse.Batch.BatchNumber;
  
            var batchLogResponse = await _assessorServiceApi.GetGetBatchLogByBatchNumber( batchNumber);

            if (batchLogResponse?.Id == null)
            {
                _aggregateLogger.LogInfo($"Could not match an existing batch Log Batch Number [{batchNumber}]");
                return;
            }

            if (!int.TryParse(batchNumber, out int batchNumberToInt))
            {
                _aggregateLogger.LogInfo($"The Batch Number is not an integer [{batchNumber}]");
                return;
            }

            var batch = new BatchData
            {
                BatchNumber = batchNumberToInt,
                BatchDate = batchResponse.Batch.BatchDate,
                PostalContactCount = batchResponse.Batch.PostalContactCount,
                TotalCertificateCount = batchResponse.Batch.TotalCertificateCount,
                PrintedDate = batchResponse.Batch.PrintedDate,
                PostedDate = batchResponse.Batch.PostedDate,
                DateOfResponse = batchResponse.Batch.DateOfResponse
            };

            await _assessorServiceApi.UpdateBatchDataInBatchLog((Guid) batchLogResponse.Id, batch);
            _fileTransferClient.DeleteFile(fileToProcess);
        }


        private async Task UploadCertificateDetailsToPinter()
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
                    var certificateFileName =
                        $"IFA-Certificate-{DateTime.UtcNow.UtcToTimeZoneTime():MMyy}-{batchNumber.ToString().PadLeft(3, '0')}.json";

                    var excelFileName = $"IFA-Certificate-{DateTime.UtcNow.UtcToTimeZoneTime()}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx";

                    var batchLogRequest = new CreateBatchLogRequest
                    {
                        BatchNumber = batchNumber,
                        FileUploadStartTime = DateTime.UtcNow,
                        Period = DateTime.UtcNow.UtcToTimeZoneTime().ToString("MMyy"),
                        BatchCreated = DateTime.UtcNow,
                        CertificatesFileName = certificateFileName
                    };

                    var configuration = ConfigurationHelper.GetConfiguration();

                    if (configuration.Sftp.UseJson)
                    {
                        _printingJsonCreator.Create(batchNumber, certificates, certificateFileName);
                        await _notificationService.Send(batchNumber, certificates, certificateFileName);

                    }
                    else
                    {
                        _printingSpreadsheetCreator.Create(batchNumber, certificates);
                        await _notificationService.Send(batchNumber, certificates, excelFileName);

                    }


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

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentDateTime;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;

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
        private readonly ISchedulingConfigurationService _schedulingConfigurationService;
        private readonly IDateTimeZoneInformation _dateTimeZoneInformation;
        private readonly IFileTransferClient _fileTransferClient;

        public PrintProcessFlowCommand(IAggregateLogger aggregateLogger,
            ISanitiserService sanitiserService,
            ICoverLetterService coverLetterService,
            IIFACertificateService ifaCertificateService,
            IAssessorServiceApi assessorServiceApi,
            INotificationService notificationService,
            ISchedulingConfigurationService schedulingConfigurationService,
            IDateTimeZoneInformation dateTimeZoneInformation,
            IFileTransferClient fileTransferClient)
        {
            _aggregateLogger = aggregateLogger;
            _sanitiserService = sanitiserService;
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _assessorServiceApi = assessorServiceApi;
            _notificationService = notificationService;
            _schedulingConfigurationService = schedulingConfigurationService;
            _dateTimeZoneInformation = dateTimeZoneInformation;
            _fileTransferClient = fileTransferClient;
        }

        public async Task Execute()
        {
            try
            {
                _aggregateLogger.LogInfo("Print Pocess Flow Function Started");
                _aggregateLogger.LogInfo("Print Function Flow Started");

                _aggregateLogger.LogInfo("Accessing Environment variables");
                _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

                _dateTimeZoneInformation.GetCurrentTimeZone();

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
                            FileUploadStartTime = DateTime.UtcNow,
                            Period = DateTime.UtcNow.UtcToTimeZoneTime().ToString("MMyy"),
                            BatchCreated = DateTime.UtcNow,
                            CertificatesFileName =
                                $"IFA-Certificate-{DateTime.UtcNow.UtcToTimeZoneTime():MMyy}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx"
                        };

                        var coverLettersProduced =
                            await _coverLetterService.Create(batchNumber, sanitizedCertificateResponses.AsEnumerable());
                        await _ifaCertificateService.Create(batchNumber, sanitizedCertificateResponses,
                            coverLettersProduced);

                        await _notificationService.Send(batchNumber, sanitizedCertificateResponses,
                            coverLettersProduced);

                        batchLogRequest.FileUploadEndTime = DateTime.UtcNow;
                        batchLogRequest.NumberOfCertificates = coverLettersProduced.CoverLetterCertificates.Count;
                        batchLogRequest.NumberOfCoverLetters = coverLettersProduced.CoverLetterFileNames.Count;
                        batchLogRequest.ScheduledDate = batchLogResponse.ScheduledDate;

                        await _fileTransferClient.LogUploadDirectory();

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
                // Convert everything back to local time to do comparisons....
                var schedulingConfiguration = await _schedulingConfigurationService.GetSchedulingConfiguration();
                var schedulingConfigurationData = JsonConvert.DeserializeObject<SchedulingConfiguraionData>(schedulingConfiguration.Data);

                var todayLocalDateTime = DateTime.UtcNow.UtcToTimeZoneTime();
                var scheduledLocalDateTime = batchLogResponse.ScheduledDate.UtcToTimeZoneTime();

                // Take care of potential configuration change
                if (schedulingConfigurationData.Hour != scheduledLocalDateTime.Hour
                    || (DayOfWeek)schedulingConfigurationData.DayOfWeek != scheduledLocalDateTime.DayOfWeek
                    || schedulingConfigurationData.Minute != scheduledLocalDateTime.Minute)
                {
                    _aggregateLogger.LogInfo("Detected change in schedule config ...");
                    var diff = schedulingConfigurationData.DayOfWeek - (int)batchLogResponse.ScheduledDate.DayOfWeek;

                    var nextDate = scheduledLocalDateTime.Next((DayOfWeek)schedulingConfigurationData.DayOfWeek).Date;
                    var tempDate = todayLocalDateTime.Date > nextDate ?
                        todayLocalDateTime.Previous((DayOfWeek)schedulingConfigurationData.DayOfWeek).Date : nextDate;

                    tempDate = tempDate.AddHours(schedulingConfigurationData.Hour);
                    tempDate = tempDate.AddMinutes(schedulingConfigurationData.Minute);
                    scheduledLocalDateTime = tempDate;

                    _aggregateLogger.LogInfo($"Next scheduled date = {scheduledLocalDateTime}");
                }
                else
                {
                    scheduledLocalDateTime = scheduledLocalDateTime.AddDays(7);
                    if (todayLocalDateTime.Date > scheduledLocalDateTime.Date)
                    {
                        var tempDate = todayLocalDateTime.Next((DayOfWeek)schedulingConfigurationData.DayOfWeek).AddDays(-7).Date;
                        tempDate = tempDate.AddHours(schedulingConfigurationData.Hour);
                        tempDate = tempDate.AddMinutes(schedulingConfigurationData.Minute);
                        scheduledLocalDateTime = tempDate;
                    }
                }

                batchLogResponse.ScheduledDate = scheduledLocalDateTime.UtcFromTimeZoneTime();

                _aggregateLogger.LogInfo($"scheduledLocalDate = {scheduledLocalDateTime}");
                _aggregateLogger.LogInfo($"todayLocalDate.Date = {todayLocalDateTime}");
                _aggregateLogger.LogInfo($"batchLogResponse.ScheduledDate = {batchLogResponse.ScheduledDate}");

                if (todayLocalDateTime >= scheduledLocalDateTime)
                    return true;
            }

            return false;
        }
    }
}

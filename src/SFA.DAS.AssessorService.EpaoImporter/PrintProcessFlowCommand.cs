﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentDateTime;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
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
        private readonly ISchedulingConfigurationService _schedulingConfigurationService;

        public PrintProcessFlowCommand(IAggregateLogger aggregateLogger,
            ISanitiserService sanitiserService,
            ICoverLetterService coverLetterService,
            IIFACertificateService ifaCertificateService,
            IAssessorServiceApi assessorServiceApi,
            INotificationService notificationService,
            ISchedulingConfigurationService schedulingConfigurationService)
        {
            _aggregateLogger = aggregateLogger;
            _sanitiserService = sanitiserService;
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _assessorServiceApi = assessorServiceApi;
            _notificationService = notificationService;
            _schedulingConfigurationService = schedulingConfigurationService;
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
                            await _coverLetterService.Create(batchNumber, sanitizedCertificateResponses.AsEnumerable());
                        await _ifaCertificateService.Create(batchNumber, sanitizedCertificateResponses,
                            coverLettersProduced);

                        await _notificationService.Send(batchNumber, sanitizedCertificateResponses,
                            coverLettersProduced);

                        batchLogRequest.FileUploadEndTime = DateTime.Now;
                        batchLogRequest.NumberOfCertificates = coverLettersProduced.CoverLetterCertificates.Count;
                        batchLogRequest.NumberOfCoverLetters = coverLettersProduced.CoverLetterFileNames.Count;                    
                        batchLogRequest.ScheduledDate = batchLogResponse.ScheduledDate;

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
                var schedulingConfiguration = await _schedulingConfigurationService.GetSchedulingConfiguration();
                var schedulingConfigurationData = JsonConvert.DeserializeObject<SchedulingConfiguraionData>(schedulingConfiguration.Data);

                var today = DateTime.Now;
                var scheduledDate = batchLogResponse.ScheduledDate;
                // Take care of potential configuration change
                if (schedulingConfigurationData.Hour != batchLogResponse.ScheduledDate.Hour
                    || (DayOfWeek)schedulingConfigurationData.DayOfWeek != batchLogResponse.ScheduledDate.DayOfWeek
                    || schedulingConfigurationData.Minute != batchLogResponse.ScheduledDate.Minute)
                {
                    _aggregateLogger.LogInfo("Detected change in schedule config ...");
                    var diff = schedulingConfigurationData.DayOfWeek - (int)batchLogResponse.ScheduledDate.DayOfWeek;
                    if (diff >= 0)
                    {
                        if (today.Date >= scheduledDate.Date.AddDays(7).Date)
                        {
                            DateTime tempDate;
                            if (today.Date.DayOfWeek == (DayOfWeek) schedulingConfigurationData.DayOfWeek)
                            {
                                tempDate = today.Date;
                            }
                            else
                            {
                                tempDate = today.Previous((DayOfWeek) schedulingConfigurationData.DayOfWeek).Date;
                            }
                          
                            tempDate = tempDate.AddHours(scheduledDate.Hour);
                            tempDate = tempDate.AddMinutes(scheduledDate.Minute);
                            scheduledDate = tempDate;
                        }
                        else
                        {
                            scheduledDate = scheduledDate.AddDays(diff);
                        }
                    }
                    else
                    {
                        if (today.Date.Date >= scheduledDate.Date.AddDays(7).Date)
                        {
                            DateTime tempDate;
                            if (today.Date.DayOfWeek == (DayOfWeek) schedulingConfigurationData.DayOfWeek)
                            {
                                tempDate = today.Date;
                            }
                            else
                            {
                                tempDate = today.Previous((DayOfWeek) schedulingConfigurationData.DayOfWeek).Date;
                            }

                            tempDate = tempDate.AddHours(scheduledDate.Hour);
                            tempDate = tempDate.AddMinutes(scheduledDate.Minute);
                            scheduledDate = tempDate;
                        }
                        else
                        {
                            scheduledDate = scheduledDate.Next((DayOfWeek) schedulingConfigurationData.DayOfWeek);
                        }
                    }

                    _aggregateLogger.LogInfo($"Next scheduled date = {scheduledDate}");
                }
                else
                {
                    scheduledDate = batchLogResponse.ScheduledDate.AddDays(7);
                    if (today.Date > scheduledDate.Date)
                    {
                        var tempDate = today.Next((DayOfWeek) schedulingConfigurationData.DayOfWeek).AddDays(-7).Date;
                        tempDate = tempDate.AddHours(scheduledDate.Hour);
                        tempDate = tempDate.AddMinutes(scheduledDate.Minute);
                        scheduledDate = tempDate;
                    }
                }

                batchLogResponse.ScheduledDate = scheduledDate;

                if (today >= scheduledDate)
                    return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnumsNET;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class GetLearnerDetailHandler : IRequestHandler<GetLearnerDetailRequest, LearnerDetailResult>
    {
        private readonly ILearnerRepository _learnerRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<GetLearnerDetailHandler> _logger;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IStandardRepository _standardRepository;

        public GetLearnerDetailHandler(ILearnerRepository learnerRepository, ICertificateRepository certificateRepository,
            IStaffCertificateRepository staffCertificateRepository, ILogger<GetLearnerDetailHandler> logger, IOrganisationQueryRepository organisationRepository, IStandardRepository standardRepository)
        {
            _learnerRepository = learnerRepository;
            _certificateRepository = certificateRepository;
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
            _organisationRepository = organisationRepository;
            _standardRepository = standardRepository;
        }

        public async Task<LearnerDetailResult> Handle(GetLearnerDetailRequest request,
            CancellationToken cancellationToken)
        {
            var learner = await _learnerRepository.Get(request.Uln, request.StdCode);            
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StdCode);

            var logs = new List<CertificateLogSummary>();
            var certificateData = new CertificateData();
            var epao = new Organisation();
            if (certificate != null)
            {
                if (request.AllRecords)
                {
                    logs.AddRange(await _staffCertificateRepository.GetAllCertificateLogs(certificate.Id));
                }
                else
                {
                    var showSummaryStatus = new[] { CertificateStatus.Submitted }.Concat(CertificateStatus.PrintProcessStatus).ToList();
                    if (showSummaryStatus.Contains(certificate.Status))
                    {
                        logs.AddRange(await _staffCertificateRepository.GetSummaryCertificateLogs(certificate.Id));
                    }
                    else
                    {
                        var latestCertificateLog = await _staffCertificateRepository.GetLatestCertificateLog(certificate.Id);
                        if (latestCertificateLog != null)
                        {
                            logs.Add(latestCertificateLog);
                        }
                    }
                }

                if (logs.Count() > 1)
                {
                    CalculateDifferences(logs);
                }

                certificateData = certificate.CertificateData ?? new CertificateData();
                epao = await _organisationRepository.Get(certificate.OrganisationId) ?? new Organisation();
            }

            var standard = await _standardRepository.GetStandardVersionByLarsCode(request.StdCode, certificateData?.Version);

            var learnerDetail = new LearnerDetailResult()
            {
                Uln = request.Uln,
                FamilyName = !string.IsNullOrEmpty(certificateData.LearnerFamilyName) ? certificateData.LearnerFamilyName : learner?.FamilyName,
                GivenNames = !string.IsNullOrEmpty(certificateData.LearnerGivenNames) ? certificateData.LearnerGivenNames : learner?.GivenNames,
                LearnStartDate = certificateData.LearningStartDate.HasValue ? certificateData.LearningStartDate : learner?.LearnStartDate,
                StandardCode = (certificate?.StandardCode).HasValue ? certificate.StandardCode : standard?.LarsCode ?? 0,
                Standard = !string.IsNullOrEmpty(certificateData.StandardName) ? certificateData.StandardName : standard?.Title,
                Version = !string.IsNullOrEmpty(certificateData.Version) ? certificateData.Version : learner?.Version,
                Level = certificateData.StandardLevel > 0 ? certificateData.StandardLevel : standard?.Level ?? 0,
                CertificateReference = certificate?.CertificateReference,
                CertificateStatus = certificate?.Status,
                OverallGrade = certificateData.OverallGrade,
                AchievementDate = certificateData.AchievementDate,
                Option = !string.IsNullOrEmpty(certificateData.CourseOption) ? certificateData.CourseOption : learner?.CourseOption,
                OrganisationName = epao.EndPointAssessorName,
                OrganisationId = epao.EndPointAssessorOrganisationId,
                CertificateLogs = logs,
                FundingModel = learner?.FundingModel,
                CompletionStatus = learner?.CompletionStatus,
                CompletionStatusDescription = FormatCompletionStatusDescription(learner?.CompletionStatus),
                IsPrivatelyFunded = certificate?.IsPrivatelyFunded,
                CertificateId = certificate?.Id,
                PrintStatusAt = certificate?.CertificateBatchLog?.StatusAt,
                ReasonForChange = certificate?.CertificateBatchLog?.ReasonForChange,
                CertificateSendTo = certificateData.SendTo.ToString(),
                ContactName = certificateData.ContactName,
                ContactDept = certificateData.Department,
                ContactOrganisation = certificateData.ContactOrganisation,
                ContactAddLine1 = certificateData.ContactAddLine1,
                ContactAddLine2 = certificateData.ContactAddLine2,
                ContactAddLine3 = certificateData.ContactAddLine3,
                ContactAddLine4 = certificateData.ContactAddLine4,
                ContactPostCode = certificateData.ContactPostCode,
                LastUpdatedAt = certificate?.LatestChange()
            };

            return learnerDetail;
        }

        private string FormatCompletionStatusDescription(int? completionStatus)
        {
            switch (completionStatus)
            {
                case (int)CompletionStatus.Continuing:
                    return $"{completionStatus} - Continuing";

                case (int)CompletionStatus.Complete:
                    return $"{completionStatus} - Completed";

                case (int)CompletionStatus.Withdrawn:
                    return $"{completionStatus} - Withdrawn";

                case (int)CompletionStatus.TemporarilyWithdrawn:
                    return $"{completionStatus} - Temporarily withdrawn";

                default:
                    return string.Empty;
            }
        }

        private void CalculateDifferences(List<CertificateLogSummary> logs)
        {
            for (int i = 0; i < logs.Count(); i++)
            {
                var thisLog = logs[i];
                if (i != logs.Count() - 1)
                {
                    var prevLog = logs[i + 1];

                    thisLog.DifferencesToPrevious = GetChanges(thisLog, prevLog);
                }
                else
                {
                    thisLog.DifferencesToPrevious = new List<CertificateLogSummary.Difference>();
                }
            }
        }

        private List<CertificateLogSummary.Difference> GetChanges(CertificateLogSummary thisLog, CertificateLogSummary prevLog)
        {
            var changes = new List<CertificateLogSummary.Difference>();

            var thisData = JsonConvert.DeserializeObject<CertificateData>(thisLog.CertificateData);
            var prevData = JsonConvert.DeserializeObject<CertificateData>(prevLog.CertificateData);
            
            // do not use generic change calculation for some properties
            var ignoreProperties = new string[] { nameof(CertificateData.EpaDetails),
                nameof(CertificateData.ReprintReasons), nameof(CertificateData.AmendReasons) };

            foreach (var propertyInfo in thisData.GetType().GetProperties())
            {
                if (ignoreProperties.Contains(propertyInfo.Name))
                    continue;
                
                var propertyIsList = propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType == typeof(List<string>);

                var thisProperty = propertyInfo.GetValue(thisData)?.ToString();
                var prevProperty = propertyInfo.GetValue(prevData)?.ToString();
                
                if (prevProperty is null && thisProperty is null)
                    continue;

                if (thisProperty != prevProperty)
                {
                    if (propertyInfo.PropertyType == typeof(DateTime) && DateTime.TryParse(thisProperty, out var result))
                    {
                        thisProperty = result.UtcToTimeZoneTime().ToShortDateString();
                    }

                    changes.Add(new CertificateLogSummary.Difference 
                    { 
                        Key = propertyInfo.Name.Spaceyfy(), 
                        Values = new List<string>
                        { 
                            string.IsNullOrEmpty(thisProperty)
                                ? "<Empty>"
                                : thisProperty
                        }
                    });
                }
            }

            // always populate the incident number and reprint or amend reasons in the changes
            if (thisLog.Action == CertificateActions.ReprintReason || thisLog.Action == CertificateActions.AmendReason)
            {
                if (!changes.Exists(p => p.Key == nameof(CertificateData.IncidentNumber).Spaceyfy()))
                {
                    changes.Add(new CertificateLogSummary.Difference { Key = nameof(CertificateData.IncidentNumber).Spaceyfy(), Values = new List<string> { thisData.IncidentNumber } });
                }

                if (thisLog.Action == CertificateActions.ReprintReason)
                {
                    var reprintReasons = thisData.ReprintReasons?.Select(p => Enum.TryParse(p, out ReprintReasons reprintReason)
                        ? reprintReason.AsString(EnumFormat.Description) : p).ToList();

                    changes.Add(new CertificateLogSummary.Difference { Key = nameof(CertificateData.ReprintReasons).Spaceyfy(), Values = reprintReasons, IsList = true });
                }
                else if (thisLog.Action == CertificateActions.AmendReason)
                {
                    var amendReasons = thisData.AmendReasons?.Select(p => Enum.TryParse(p, out AmendReasons amendReason)
                        ? amendReason.AsString(EnumFormat.Description) : p).ToList();

                    changes.Add(new CertificateLogSummary.Difference { Key = nameof(CertificateData.AmendReasons).Spaceyfy(), Values = amendReasons, IsList = true });
                }
            }

            return changes;
        }
    }

    public static class StringExtensions
    {
        public static string Spaceyfy(this string target)
        {
            var spaceyfiedString = target[0].ToString();
            foreach (var character in target.Skip(1))
            {
                if (char.IsUpper(character) || char.IsNumber(character))
                {
                    spaceyfiedString += " ";
                }

                spaceyfiedString += character;
            }

            return spaceyfiedString;
        }
    }
}
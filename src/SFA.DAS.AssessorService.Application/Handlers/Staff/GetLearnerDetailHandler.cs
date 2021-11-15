using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Extensions;

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

                certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData) ?? new CertificateData();
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
                AchievementDate = certificateData.AchievementDate, //?.UtcToTimeZoneTime(),
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
                ContactName = certificateData.ContactName,
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
                    var previousLog = logs[i + 1];

                    thisLog.DifferencesToPrevious = GetDifference(thisLog.CertificateData, previousLog.CertificateData);
                }
                else
                {
                    thisLog.DifferencesToPrevious = new Dictionary<string, string>();
                }
            }
        }

        private Dictionary<string, string> GetDifference(string thisLogCertificateData, string previousLogCertificateData)
        {
            var differences = new Dictionary<string, string>();
            var thisData = JsonConvert.DeserializeObject<CertificateData>(thisLogCertificateData);
            var prevData = JsonConvert.DeserializeObject<CertificateData>(previousLogCertificateData);

            //*** DO NOT Calculate differences in EpaDetails 
            thisData.EpaDetails = null;
            prevData.EpaDetails = null;

            foreach (var propertyInfo in thisData.GetType().GetProperties())
            {
                var thisProperty = propertyInfo.GetValue(thisData)?.ToString();
                var prevProperty = propertyInfo.GetValue(prevData)?.ToString();
                if (prevProperty != thisProperty)
                {
                    var ignoreProperties = propertyInfo.Name.Equals("Version", StringComparison.OrdinalIgnoreCase);
                    if (!ignoreProperties && DateTime.TryParse(thisProperty, out var result))
                    {
                        thisProperty = result.UtcToTimeZoneTime().ToShortDateString();
                    }

                    differences.Add(propertyInfo.Name.Spaceyfy(), string.IsNullOrEmpty(thisProperty) ? "<Empty>" : thisProperty);
                }
            }

            return differences;
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
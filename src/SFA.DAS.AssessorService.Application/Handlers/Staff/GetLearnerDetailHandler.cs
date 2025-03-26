using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Extensions;
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
                        var latestCertificateLogs = await _staffCertificateRepository.GetLatestCertificateLogs(certificate.Id);
                        if (latestCertificateLogs!= null && latestCertificateLogs.Count > 0)
                        {
                            logs.Add(latestCertificateLogs.First());
                        }
                    }
                }

                if (logs.Count() > 1)
                {
                    logs.CalculateDifferences();
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
    }   
}
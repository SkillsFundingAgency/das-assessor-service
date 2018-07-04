using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class LearnerDetailHandler : IRequestHandler<LearnerDetailRequest, LearnerDetail>
    {
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _apiClient;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<LearnerDetailHandler> _logger;

        public LearnerDetailHandler(IIlrRepository ilrRepository, IAssessmentOrgsApiClient apiClient, ICertificateRepository certificateRepository, IStaffCertificateRepository staffCertificateRepository, ILogger<LearnerDetailHandler> logger)
        {
            _ilrRepository = ilrRepository;
            _apiClient = apiClient;
            _certificateRepository = certificateRepository;
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
        }
        public async Task<LearnerDetail> Handle(LearnerDetailRequest request, CancellationToken cancellationToken)
        {
            var learner = await _ilrRepository.Get(request.Uln, request.StdCode);
            var standard = await _apiClient.GetStandard(learner.StdCode);
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StdCode);

            var logs = new List<CertificateLogSummary>();
            var certificateData = new CertificateData();
            if (certificate != null)
            {
                logs = await _staffCertificateRepository.GetCertificateLogsFor(certificate.Id);
                certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            }
            
            var learnerDetail = new LearnerDetail()
            {
                Uln = learner.Uln,
                FamilyName = learner.FamilyName,
                GivenNames = learner.GivenNames,
                LearnStartDate = learner.LearnStartDate,
                Standard = standard.Title,
                CertificateReference = certificate?.CertificateReference, 
                CertificateStatus = certificate?.Status, 
                Level = standard.Level,
                OverallGrade = certificateData.OverallGrade,
                AchievementDate = certificateData.AchievementDate?.ToLocalTime(),

                CertificateLogs = logs
            };

            return learnerDetail;
        }
    }
}
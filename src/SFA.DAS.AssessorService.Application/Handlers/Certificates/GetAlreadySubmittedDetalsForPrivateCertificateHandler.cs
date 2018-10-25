using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    public class GetAlreadySubmittedDetalsForPrivateCertificateHandler : IRequestHandler<GetPrivateCertificateAlreadySubmittedRequest, GetPrivateCertificateAlreadySubmittedResponse>
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
      
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SearchHandler> _logger;
      

        public GetAlreadySubmittedDetalsForPrivateCertificateHandler(
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ICertificateApiClient certificateApiClient,
            IOrganisationQueryRepository organisationRepository,        
            ICertificateRepository certificateRepository, 
            ILogger<SearchHandler> logger)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _certificateApiClient = certificateApiClient;
            _organisationRepository = organisationRepository;
            
            _certificateRepository = certificateRepository;
            _logger = logger;            
        }

        public async Task<GetPrivateCertificateAlreadySubmittedResponse> Handle(GetPrivateCertificateAlreadySubmittedRequest request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.Id);
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            var certificateLogs = await _certificateRepository.GetCertificateLogsFor(certificate.Id);
            var submittedLog = certificateLogs.FirstOrDefault(q => q.Status == CertificateStatus.Submitted);
            var submittedBy = submittedLog.Username;
            var submittedAt = submittedLog.EventTime;
            
            var standard = await _assessmentOrgsApiClient.GetStandard(certificate.StandardCode);

            var certificateAlreadySubmittedResponse = new GetPrivateCertificateAlreadySubmittedResponse
            {
                Standard = standard.Title,
                StdCode = certificate.StandardCode.ToString(),
                Uln = certificate.Uln.ToString(),
                GivenNames = certificateData.LearnerGivenNames,
                FamilyName = certificateData.LearnerFamilyName,
                CertificateReference = certificate.CertificateReference,
                OverallGrade = certificateData.OverallGrade,
                Level = certificateData.StandardLevel.ToString(),
                SubmittedAt = GetSubmittedAtString(submittedAt),
                SubmittedBy = submittedBy,
                LearnerStartDate = certificateData.LearningStartDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                AchievementDate = certificateData.AchievementDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                ShowExtraInfo = true
            };

          

            return certificateAlreadySubmittedResponse;
        }
        
        private string GetSubmittedAtString(DateTime? submittedAt)
        {
            return !submittedAt.HasValue ? "" : submittedAt.Value.ToString("d MMMM yyyy");
        }      
    }
}
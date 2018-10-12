using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
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
        private readonly IOrganisationQueryRepository _organisationRepository;

        public LearnerDetailHandler(IIlrRepository ilrRepository, IAssessmentOrgsApiClient apiClient, ICertificateRepository certificateRepository, 
            IStaffCertificateRepository staffCertificateRepository, ILogger<LearnerDetailHandler> logger, IOrganisationQueryRepository organisationRepository)
        {
            _ilrRepository = ilrRepository;
            _apiClient = apiClient;
            _certificateRepository = certificateRepository;
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
            _organisationRepository = organisationRepository;
        }
        public async Task<LearnerDetail> Handle(LearnerDetailRequest request, CancellationToken cancellationToken)
        {
            var learner = await _ilrRepository.Get(request.Uln, request.StdCode);
            var standard = await _apiClient.GetStandard(learner.StdCode);
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StdCode);

            var logs = new List<CertificateLogSummary>();
            var certificateData = new CertificateData();
            var epao = new Organisation();
            if (certificate != null)
            {
                logs = await _staffCertificateRepository.GetCertificateLogsFor(certificate.Id,
                    request.AllRecords);
                if (logs.Count() > 1)
                {
                    CalculateDifferences(logs);
                }
                

                certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                epao = await _organisationRepository.Get(certificate.OrganisationId);
            }
            
            var learnerDetail = new LearnerDetail()
            {
                Uln = learner.Uln,
                FamilyName = certificateData.LearnerFamilyName ?? learner.FamilyName,
                GivenNames = certificateData.LearnerGivenNames ?? learner.GivenNames,
                LearnStartDate = learner.LearnStartDate,
                StandardCode = learner.StdCode,
                Standard = standard.Title,
                CertificateReference = certificate?.CertificateReference, 
                CertificateStatus = certificate?.Status, 
                Level = standard.Level,
                OverallGrade = certificateData.OverallGrade,
                AchievementDate = certificateData.AchievementDate, //?.UtcToTimeZoneTime(),
                Option = certificateData.CourseOption, 
                OrganisationName = epao.EndPointAssessorName,
                OrganisationId = epao.EndPointAssessorOrganisationId,
                CertificateLogs = logs,
                FundingModel = learner.FundingModel,
                CertificateId = certificate?.Id
            };

            return learnerDetail;
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

            foreach (var propertyInfo in thisData.GetType().GetProperties())
            {
                var thisProperty = propertyInfo.GetValue(thisData)?.ToString();
                var prevProperty = propertyInfo.GetValue(prevData)?.ToString();
                if (prevProperty != thisProperty)
                {
                    if (DateTime.TryParse(thisProperty, out var result))
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
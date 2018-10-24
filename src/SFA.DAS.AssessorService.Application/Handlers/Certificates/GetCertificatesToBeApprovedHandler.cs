using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.ExternalApis;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesToBeApprovedHandler : IRequestHandler<GetToBeApprovedCertificatesRequest,
        List<CertificateSummaryResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly ILogger<GetCertificatesHistoryHandler> _logger;

        public GetCertificatesToBeApprovedHandler(ICertificateRepository certificateRepository,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ILogger<GetCertificatesHistoryHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _logger = logger;
        }

        public async Task<List<CertificateSummaryResponse>> Handle(GetToBeApprovedCertificatesRequest request,
            CancellationToken cancellationToken)
        {
            var statuses = new List<string>
            {                
                Domain.Consts.CertificateStatus.ToBeApproved,
                Domain.Consts.CertificateStatus.Approved,
                Domain.Consts.CertificateStatus.Rejected,
                Domain.Consts.CertificateStatus.SentForApproval
            };

            var certificates = await _certificateRepository.GetCertificates(statuses);

            // Please Note:- Cannot seem to automap this with custom value/type converters
            // so dealing with it manually for now.
            var approvals = MapCertificates(certificates);
            return approvals;
        }

        private List<CertificateSummaryResponse> MapCertificates(IEnumerable<Certificate> certificates)
        {
            var trainingProviderName = string.Empty;
            var recordedBy = string.Empty;

            var certificateResponses = certificates.Select(
                certificate =>
                {
                    var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                    recordedBy = certificate.CertificateLogs
                        .OrderByDescending(q => q.EventTime)
                        .FirstOrDefault(certificateLog =>
                            certificateLog.Status == Domain.Consts.CertificateStatus.Submitted)?.Username;

                    try
                    {
                        if (certificateData.ProviderName == null)
                        {
                            if (certificate.ProviderUkPrn == 0)
                            {
                                _logger.LogInformation(
                                    $"Cannot find training provider for ukprn {certificate.Organisation.EndPointAssessorUkprn.Value}");                                
                            }
                            else
                            {
                                var provider = _assessmentOrgsApiClient
                                    .GetProvider(certificate.ProviderUkPrn).GetAwaiter()
                                    .GetResult();

                                trainingProviderName = provider.ProviderName;
                                _certificateRepository.UpdateProviderName(certificate.Id, trainingProviderName);
                            }
                        }
                        else
                        {
                            trainingProviderName = certificateData.ProviderName;
                        }
                    }
                    catch (EntityNotFoundException)
                    {
                        _logger.LogInformation(
                            $"Cannot find training provider for ukprn {certificate.Organisation.EndPointAssessorUkprn.Value}");
                    }

                    return new CertificateSummaryResponse
                    {
                        CertificateReference = certificate.CertificateReference,
                        Uln = certificate.Uln,
                        CreatedAt = certificate.CreatedAt,
                        ContactOrganisation = certificateData.ContactOrganisation,
                        ContactName = certificateData.ContactName,
                        TrainingProvider = trainingProviderName,
                        RecordedBy = recordedBy,
                        CourseOption = certificateData.CourseOption,
                        FullName = certificateData.FullName,
                        OverallGrade = certificateData.OverallGrade,
                        StandardName = certificateData.StandardName,
                        Level = certificateData.StandardLevel,
                        AchievementDate = certificateData.AchievementDate,
                        LearningStartDate = certificateData.LearningStartDate,
                        ContactAddLine1 = certificateData.ContactAddLine1,
                        ContactAddLine2 = certificateData.ContactAddLine2,
                        ContactAddLine3 = certificateData.ContactAddLine3,
                        ContactAddLine4 = certificateData.ContactAddLine4,
                        ContactPostCode = certificateData.ContactPostCode,
                        Status = certificate.Status
                    };
                });

            var responses = certificateResponses.ToList();
            return responses;
        }
    }
}
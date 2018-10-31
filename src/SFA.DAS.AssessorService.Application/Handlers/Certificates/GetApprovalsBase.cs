using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetApprovalsBase<T>
    {
        protected readonly ICertificateRepository CertificateRepository;
        protected readonly IAssessmentOrgsApiClient AssessmentOrgsApiClient;
        protected readonly ILogger<T> Logger;


        public GetApprovalsBase(ICertificateRepository certificateRepository,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ILogger<T> logger)
        {
            CertificateRepository = certificateRepository;
            AssessmentOrgsApiClient = assessmentOrgsApiClient;
            Logger = logger;
        }

        protected List<CertificateSummaryResponse> MapCertificates(IEnumerable<Certificate> certificates)
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
                                Logger.LogInformation(
                                    $"Cannot find training provider for ukprn {certificate.Organisation.EndPointAssessorUkprn.Value}");
                            }
                            else
                            {
                                var provider = AssessmentOrgsApiClient
                                    .GetProvider(certificate.ProviderUkPrn).GetAwaiter()
                                    .GetResult();

                                trainingProviderName = provider.ProviderName;
                                CertificateRepository.UpdateProviderName(certificate.Id, trainingProviderName);
                            }
                        }
                        else
                        {
                            trainingProviderName = certificateData.ProviderName;
                        }
                    }
                    catch (EntityNotFoundException)
                    {
                        Logger.LogInformation(
                            $"Cannot find training provider for ukprn {certificate.Organisation.EndPointAssessorUkprn.Value}");
                    }

                    return new CertificateSummaryResponse
                    {
                        Id = certificate.Id,
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

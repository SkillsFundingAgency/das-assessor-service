using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates.Batch
{
    public class GetBatchCertificateHandler : IRequestHandler<GetBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly ILogger<GetBatchCertificateHandler> _logger;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;

        public GetBatchCertificateHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient, ILogger<GetBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _logger = logger;
        }

        public async Task<Certificate> Handle(GetBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await GetCertificate(request);
        }

        private async Task<Certificate> GetCertificate(GetBatchCertificateRequest request)
        {
            _logger.LogInformation("GetCertificate Before Get Certificate from db");
            Certificate certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate != null)
            {
                var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

                if(certData.LearnerFamilyName == request.FamilyName)
                {
                    var searchingContact = await _contactQueryRepository.GetContactFromEmailAddress(request.Email);
                    var certificateContact = await GetContactFromCertificateLogs(certificate.Id, certificate.UpdatedBy, certificate.CreatedBy);

                    if (certificateContact is null || certificateContact.OrganisationId != searchingContact.OrganisationId)
                    {
                        var providedStandards = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(searchingContact.EndPointAssessorOrganisationId);

                        if (providedStandards.Any(s => s.StandardCode == certificate.StandardCode.ToString()))
                        {
                            CertificateData redactedData = new CertificateData
                            {
                                LearnerGivenNames = certData.LearnerGivenNames,
                                LearnerFamilyName = certData.LearnerFamilyName,
                                StandardName = certData.StandardName,
                                StandardLevel = certData.StandardLevel,
                                StandardPublicationDate = certData.StandardPublicationDate
                            };

                            certificate.CertificateData = JsonConvert.SerializeObject(redactedData);
                            certificate.CreateDay = DateTime.MinValue;
                            certificate.CreatedAt = DateTime.MinValue;
                            certificate.CreatedBy = null;
                            certificate.UpdatedBy = null;
                            certificate.UpdatedAt = null;
                            certificate.DeletedBy = null;
                            certificate.DeletedAt = null;
                        }
                        else
                        {
                            certificate = null;
                        }
                    }
                }
                else
                {
                    certificate = null;
                }
            }
            else
            {
                certificate = null;
            }

            return certificate;
        }

        private async Task<Contact> GetContactFromCertificateLogs(Guid certificateId, string fallbackUpdatedUsername, string fallbackCreatedUsername)
        {
            Contact contact = null;

            _logger.LogInformation("GetContactFromCertificateLogs Before GetCertificateLogsFor");
            var certificateLogs = await _certificateRepository.GetCertificateLogsFor(certificateId);

            var submittedLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == Domain.Consts.CertificateStatus.Submitted);
            var createdLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == Domain.Consts.CertificateStatus.Draft);

            if (submittedLogEntry != null)
            {
                _logger.LogInformation("GetContactFromCertificateLogs Before Submitted LogEntry GetContact");
                contact = await _contactQueryRepository.GetContact(submittedLogEntry.Username);
            }

            if (contact is null && !string.IsNullOrEmpty(fallbackUpdatedUsername))
            {
                _logger.LogInformation("GetContactFromCertificateLogs Before Submitted Fallback GetContact");
                contact = await _contactQueryRepository.GetContact(fallbackUpdatedUsername);
            }

            if (contact is null && createdLogEntry != null)
            {
                _logger.LogInformation("GetContactFromCertificateLogs Before Created LogEntry GetContact");
                contact = await _contactQueryRepository.GetContact(createdLogEntry.Username);
            }

            if(contact is null && !string.IsNullOrEmpty(fallbackCreatedUsername))
            {
                _logger.LogInformation("GetContactFromCertificateLogs Before Created Fallback GetContact");
                contact = await _contactQueryRepository.GetContact(fallbackCreatedUsername);
            }

            return contact;
        }
    }
}

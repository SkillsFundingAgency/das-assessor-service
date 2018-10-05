using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
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

        public GetBatchCertificateHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, IIlrRepository ilrRepository, ILogger<GetBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _ilrRepository = ilrRepository;
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

            if (certificate != null && certificate.CertificateReference == request.CertificateReference)
            {
                var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

                if(certData.LearnerFamilyName == request.FamilyName)
                {
                    var searchingContact = await _contactQueryRepository.GetContactFromEmailAddress(request.Email);
                    var certificateContact = await GetContactFromCertificateLogs(certificate.Id, certificate.CreatedBy);
                    
                    if (certificateContact is null || certificateContact.OrganisationId != searchingContact.OrganisationId)
                    {
                        certData.OverallGrade = "";
                        certData.LearningStartDate = DateTime.MinValue;
                        certData.AchievementDate = null;

                        certificate.CreateDay = DateTime.MinValue;
                        certificate.CreatedAt = DateTime.MinValue;
                        certificate.CreatedBy = null;
                        certificate.UpdatedBy = null;
                        certificate.UpdatedAt = null;
                        certificate.DeletedBy = null;
                        certificate.DeletedAt = null;
                    }

                    certificate.CertificateData = JsonConvert.SerializeObject(certData);
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

        private async Task<Contact> GetContactFromCertificateLogs(Guid certificateId, string fallbackUsername)
        {
            Contact contact;

            _logger.LogInformation("GetContactFromCertificateLogs Before GetCertificateLogsFor");
            var certificateLogs = await _certificateRepository.GetCertificateLogsFor(certificateId);

            var submittedLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == Domain.Consts.CertificateStatus.Submitted);
            var createdLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == Domain.Consts.CertificateStatus.Draft);

            _logger.LogInformation("GetContactFromCertificateLogs Before GetContact");
            if (submittedLogEntry != null)
            {
                contact = await _contactQueryRepository.GetContact(submittedLogEntry.Username);
            }
            else if (createdLogEntry != null)
            {
                contact = await _contactQueryRepository.GetContact(createdLogEntry.Username);
            }
            else
            {
                contact = await _contactQueryRepository.GetContact(fallbackUsername);
            }

            return contact;
        }
    }
}

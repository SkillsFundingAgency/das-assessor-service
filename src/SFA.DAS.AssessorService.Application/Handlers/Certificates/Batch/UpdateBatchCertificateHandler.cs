using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates.Batch
{
    public class UpdateBatchCertificateHandler : IRequestHandler<UpdateBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<UpdateBatchCertificateHandler> _logger;

        public UpdateBatchCertificateHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<UpdateBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(UpdateBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await UpdateCertificate(request);
        }

        private async Task<Certificate> UpdateCertificate(UpdateBatchCertificateRequest request)
        {
            _logger.LogInformation("UpdateCertificate Before Get Contact from db");
            var contact = await GetContactFromEmailAddress(request.Email);

            _logger.LogInformation("UpdateCertificate Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);
            var certData = CombineCertificateData(JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData), request.CertificateData);

            _logger.LogInformation("UpdateCertificate Before Update CertificateData");
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            if(certificate.Status == CertificateStatus.Deleted)
            {
                certificate.Status = CertificateStatus.Draft;
            }

            _logger.LogInformation("UpdateCertificate Before Update Cert in db");
            await _certificateRepository.Update(certificate, contact.Username, CertificateActions.Amend);

            return await ApplyStatusInformation(certificate);
        }

        private async Task<Contact> GetContactFromEmailAddress(string email)
        {
            Contact contact = await _contactQueryRepository.GetContactFromEmailAddress(email);

            if (contact == null)
            {
                contact = new Contact { Username = email, Email = email };
            }

            return contact;
        }

        private CertificateData CombineCertificateData(CertificateData certData, CertificateData requestData)
        {
            return new CertificateData()
            {
                LearnerGivenNames = certData.LearnerGivenNames,
                LearnerFamilyName = certData.LearnerFamilyName,
                LearningStartDate = certData.LearningStartDate,
                StandardName = certData.StandardName,     
                StandardLevel = certData.StandardLevel,
                StandardPublicationDate = certData.StandardPublicationDate,
                FullName = certData.FullName,
                ProviderName = certData.ProviderName,

                ContactName = requestData.ContactName,
                ContactOrganisation = requestData.ContactOrganisation,
                Department = requestData.Department,
                ContactAddLine1 = requestData.ContactAddLine1,
                ContactAddLine2 = requestData.ContactAddLine2,
                ContactAddLine3 = requestData.ContactAddLine3,
                ContactAddLine4 = requestData.ContactAddLine4,
                ContactPostCode = requestData.ContactPostCode,
                Registration = requestData.Registration,
                AchievementDate = requestData.AchievementDate,
                CourseOption = requestData.CourseOption,
                OverallGrade = requestData.OverallGrade                
            };
        }

        private async Task<Certificate> ApplyStatusInformation(Certificate certificate)
        {
            var json = JsonConvert.SerializeObject(certificate);
            var cert = JsonConvert.DeserializeObject<Certificate>(json);

            var certificateLogs = await _certificateRepository.GetCertificateLogsFor(cert.Id);
            certificateLogs = certificateLogs?.Where(l => l.ReasonForChange is null).ToList(); // this removes any admin changes done within staff app

            var createdLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Draft);
            if (createdLogEntry != null)
            {
                var createdContact = await _contactQueryRepository.GetContact(createdLogEntry.Username);
                cert.CreatedAt = createdLogEntry.EventTime.UtcToTimeZoneTime();
                cert.CreatedBy = createdContact != null ? createdContact.DisplayName : createdLogEntry.Username;
            }

            var submittedLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);

            // NOTE: THIS IS A DATA FRIG FOR EXTERNAL API AS WE NEED SUBMITTED INFORMATION!
            if (submittedLogEntry != null)
            {
                var submittedContact = await _contactQueryRepository.GetContact(submittedLogEntry.Username);
                cert.UpdatedAt = submittedLogEntry.EventTime.UtcToTimeZoneTime();
                cert.UpdatedBy = submittedContact != null ? submittedContact.DisplayName : submittedLogEntry.Username;
            }
            else
            {
                cert.UpdatedAt = null;
                cert.UpdatedBy = null;
            }

            return cert;
        }
    }
}

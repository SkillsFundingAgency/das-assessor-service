using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates
{
    public class UpdateBatchCertificateHandler : IRequestHandler<UpdateBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<UpdateBatchCertificateHandler> _logger;
        private readonly IStandardService _standardService;

        public UpdateBatchCertificateHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<UpdateBatchCertificateHandler> logger, IStandardService standardService)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _standardService = standardService;
        }

        public async Task<Certificate> Handle(UpdateBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await UpdateCertificate(request);
        }

        private async Task<Certificate> UpdateCertificate(UpdateBatchCertificateRequest request)
        {
            _logger.LogInformation("UpdateCertificate Before Get Contact from db");
            var contact = await GetContactFromEmailAddress(request.Email);

            _logger.LogInformation("UpdateCertificate Before Get Standard from API");
            var standard = await _standardService.GetStandard(request.StandardCode);

            _logger.LogInformation("UpdateCertificate Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);
            var certData = CombineCertificateData(JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData), request.CertificateData, standard);

            _logger.LogInformation("UpdateCertificate Before Update CertificateData");
            
            // need to update EPA Reference too
            certData.EpaDetails.EpaReference = certificate.CertificateReference;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            // adjust Status appropriately
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

        private CertificateData CombineCertificateData(CertificateData certData, CertificateData requestData, StandardCollation standard)
        {
            var epaDetails = certData.EpaDetails ?? new EpaDetails();
            if (epaDetails.Epas is null) epaDetails.Epas = new List<EpaRecord>();

            var epaOutcome = certData.OverallGrade == CertificateGrade.Fail ? EpaOutcome.Fail : EpaOutcome.Pass;
            if (requestData.AchievementDate != null && !epaDetails.Epas.Any(rec => rec.EpaDate == requestData.AchievementDate.Value && rec.EpaOutcome == epaOutcome))
            {
                var record = new EpaRecord { EpaDate = requestData.AchievementDate.Value, EpaOutcome = epaOutcome };
                epaDetails.Epas.Add(record);

                var latestRecord = epaDetails.Epas.OrderByDescending(epa => epa.EpaDate).First();
                epaDetails.LatestEpaDate = latestRecord.EpaDate;
                epaDetails.LatestEpaOutcome = latestRecord.EpaOutcome;
            }

            return new CertificateData()
            {
                LearnerGivenNames = certData.LearnerGivenNames,
                LearnerFamilyName = certData.LearnerFamilyName,
                LearningStartDate = certData.LearningStartDate,
                StandardReference = certData.StandardReference,
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
                CourseOption = NormalizeCourseOption(requestData.CourseOption, standard),
                OverallGrade = NormalizeOverallGrade(requestData.OverallGrade),

                EpaDetails = epaDetails
            };
        }

        private async Task<Certificate> ApplyStatusInformation(Certificate certificate)
        {
            var json = JsonConvert.SerializeObject(certificate);
            var cert = JsonConvert.DeserializeObject<Certificate>(json);

            var certificateLogs = await _certificateRepository.GetCertificateLogsFor(cert.Id);
            certificateLogs = certificateLogs?.Where(l => l.ReasonForChange is null).ToList(); // this removes any admin changes done within staff app

            var createdLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == CertificateStatus.Draft);
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

        private static string NormalizeOverallGrade(string overallGrade)
        {
            var grades = new string[] { CertificateGrade.Pass, CertificateGrade.Credit, CertificateGrade.Merit, CertificateGrade.Distinction, CertificateGrade.PassWithExcellence, CertificateGrade.NoGradeAwarded };
            return grades.FirstOrDefault(g => g.Equals(overallGrade, StringComparison.InvariantCultureIgnoreCase)) ?? overallGrade;
        }

        private static string NormalizeCourseOption(string courseOption, StandardCollation standard)
        {
            if (standard.Options is null)
            {
                return courseOption;
            }
            else
            {
                return standard.Options.FirstOrDefault(g => g.Equals(courseOption, StringComparison.InvariantCultureIgnoreCase)) ?? courseOption;
            }
        }
    }
}

using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses
{
    internal static class CertificateHelpers
    {
        public static async Task<Certificate> ApplyStatusInformation(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, Certificate certificate)
        {
            if (certificateRepository is null || contactQueryRepository is null || certificate is null) return certificate;

            var json = JsonConvert.SerializeObject(certificate);
            var cert = JsonConvert.DeserializeObject<Certificate>(json);

            var certificateLogs = await certificateRepository.GetCertificateLogsFor(cert.Id);
            certificateLogs = certificateLogs?.Where(l => l.ReasonForChange is null).ToList(); // this removes any admin changes done within staff app

            var createdLogEntry = certificateLogs?.OrderByDescending(l => l.EventTime).FirstOrDefault(l => l.Status == CertificateStatus.Draft && l.Action == CertificateActions.Start);
            if (createdLogEntry != null)
            {
                var createdContact = await contactQueryRepository.GetContact(createdLogEntry.Username);
                cert.CreatedAt = createdLogEntry.EventTime.UtcToTimeZoneTime();
                cert.CreatedBy = createdContact != null ? createdContact.DisplayName : createdLogEntry.Username;
            }

            var submittedLogEntry = certificateLogs?.OrderByDescending(l => l.EventTime).FirstOrDefault(l => l.Status == CertificateStatus.Submitted);

            // NOTE: THIS IS A DATA FRIG FOR EXTERNAL API AS WE NEED SUBMITTED INFORMATION!
            // Amended, don't return submitted info, if the status has returned to draft after a fail.
            if (submittedLogEntry != null && certificate.Status != CertificateStatus.Draft)
            {
                var submittedContact = await contactQueryRepository.GetContact(submittedLogEntry.Username);
                cert.UpdatedAt = submittedLogEntry.EventTime.UtcToTimeZoneTime();
                cert.UpdatedBy = submittedContact != null ? submittedContact.DisplayName : submittedLogEntry.Username;
            }
            else
            {
                cert.UpdatedAt = null;
                cert.UpdatedBy = null;
            }

            // Remove Print & Batch information if going for a Reprint
            if (cert.Status == CertificateStatus.Reprint)
            {
                cert.ToBePrinted = null;
                cert.BatchNumber = null;
            }

            return cert;
        }

        public static string NormalizeOverallGrade(string overallGrade)
        {
            var grades = new string[] { CertificateGrade.Pass, CertificateGrade.Credit, CertificateGrade.Merit, CertificateGrade.Distinction, CertificateGrade.PassWithExcellence, CertificateGrade.Outstanding, CertificateGrade.NoGradeAwarded };
            return grades.FirstOrDefault(g => g.Equals(overallGrade, StringComparison.InvariantCultureIgnoreCase)) ?? overallGrade;
        }

        public static string NormalizeCourseOption(StandardOptions standardOptions, string courseOption)
        {
            if (standardOptions is null || !standardOptions.HasOptions())
            {
                return courseOption;
            }
            else
            {
                return standardOptions.CourseOption.FirstOrDefault(g => g.Equals(courseOption, StringComparison.InvariantCultureIgnoreCase)) ?? courseOption;
            }
        }
    }
}

using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.External.Helpers
{
    public static class CertificateHelpers
    {
        public static bool IsDraftCertificateDeemedAsReady(Certificate certificate, IEnumerable<string> potentialOptions = null)
        {
            // Note: This allows the caller to know if a Draft Certificate is 'Ready' for submitting
            // It is deemed ready if the mandatory fields have been filled out.
            if (certificate?.CertificateData is null || certificate?.Status?.CurrentStatus != CertificateStatus.Draft || string.IsNullOrEmpty(certificate.CertificateData.CertificateReference))
            {
                return false;
            }
            else if (certificate.CertificateData.Standard is null || certificate.CertificateData.Standard.StandardCode < 1)
            {
                return false;
            }
            else if (certificate.CertificateData.LearningDetails.Version is null)
            {
                return false;
            }
            else if (certificate.CertificateData.PostalContact is null 
                    || string.IsNullOrEmpty(certificate.CertificateData.PostalContact.ContactName)
                    || string.IsNullOrEmpty(certificate.CertificateData.PostalContact.City) 
                    || string.IsNullOrEmpty(certificate.CertificateData.PostalContact.PostCode))
            {
                return false;
            }
            else if (certificate.CertificateData.Learner is null || string.IsNullOrEmpty(certificate.CertificateData.Learner.FamilyName)
//                      || certificate.CertificateData.Learner.Uln < 1000000000 || certificate.CertificateData.Learner.Uln > 9999999999)
                        || certificate.CertificateData.Learner.Uln <= 1000000000 || certificate.CertificateData.Learner.Uln >= 9999999999)
            {
                return false;
            }
            else if (certificate.CertificateData.LearningDetails is null || string.IsNullOrEmpty(certificate.CertificateData.LearningDetails.OverallGrade)
                        || !certificate.CertificateData.LearningDetails.AchievementDate.HasValue)
            {
                return false;
            } 
            else if (potentialOptions != null && potentialOptions.Any())
            {
                if (certificate.CertificateData.LearningDetails.CourseOption is null ) //|| !potentialOptions.Contains(certificate.CertificateData.LearningDetails.CourseOption))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Linq;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateVersionViewModel : CertificateBaseViewModel
    {
        public IEnumerable<StandardVersionViewModel> Versions { get; set; }

        public StandardVersion SelectedStandardVersion { get; set; }
        public StandardOptions SelectedStandardOptions { get; set; }
        public CertificateData PreviousFailData { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert, IEnumerable<StandardVersionViewModel> versions)
        {
            base.FromCertificate(cert);
            Versions = versions.OrderBy(v => v.Version);
            PreviousFailData = GetLatestFailData(cert);
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certData.StandardReference = SelectedStandardVersion.IFateReferenceNumber;
            certData.StandardName = SelectedStandardVersion.Title;
            certData.StandardLevel = SelectedStandardVersion.Level;
            certData.StandardPublicationDate = SelectedStandardVersion.EffectiveFrom;
            certData.Version = SelectedStandardVersion.Version;

            if (SelectedStandardOptions != null && SelectedStandardOptions.OnlyOneOption())
            {
                // If only one option set on the certificate
                certData.CourseOption = SelectedStandardOptions.CourseOption.Single();
            }
            else if(certificate.StandardUId != StandardUId)
            {
                // If changed, wipe the option in case different versions have different options
                certData.CourseOption = null;
            }

            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            certificate.StandardUId = StandardUId;
            return certificate;
        }

        private CertificateData GetLatestFailData(Domain.Entities.Certificate cert)
        {
            var submitLogs = cert.CertificateLogs.Where(log => log.Action == "Submit");

            var submittedCertificates = submitLogs.Select(log => JsonConvert.DeserializeObject<CertificateData>(log.CertificateData));

            var latestSubmittedFailCertificate = submittedCertificates.Where(submittedCert => submittedCert.OverallGrade == CertificateGrade.Fail)
                .OrderByDescending(submittedCert => submittedCert.AchievementDate)
                .FirstOrDefault();

            return latestSubmittedFailCertificate;
        }
    }
}
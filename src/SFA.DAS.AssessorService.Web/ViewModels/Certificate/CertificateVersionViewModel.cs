using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Linq;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    using Certificate = Domain.Entities.Certificate;

    public class CertificateVersionViewModel : CertificateBaseViewModel
    {
        public IEnumerable<StandardVersionViewModel> Versions { get; set; }

        public StandardVersion SelectedStandardVersion { get; set; }
        public StandardOptions SelectedStandardOptions { get; set; }
        public CertificateData PreviousFailData { get; set; }

        public void FromCertificate(Certificate cert, IEnumerable<StandardVersionViewModel> versions)
        {
            base.FromCertificate(cert);
            Versions = versions.OrderBy(v => v.Version);
            PreviousFailData = GetLatestFailData(cert);
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.StandardReference = SelectedStandardVersion.IFateReferenceNumber;
            certData.StandardName = SelectedStandardVersion.Title;
            certData.StandardLevel = SelectedStandardVersion.Level;
            certData.StandardPublicationDate = SelectedStandardVersion.EffectiveFrom;
            certData.Version = SelectedStandardVersion.Version;
            certData.CoronationEmblem = SelectedStandardVersion.CoronationEmblem;

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

            certificate.CertificateData = certData;
            certificate.StandardUId = StandardUId;
            return certificate;
        }

        private CertificateData GetLatestFailData(Domain.Entities.Certificate cert)
        {
            var submitLogs = cert.CertificateLogs.Where(log => log.Action == "Submit");

            var submittedCertificates = submitLogs.Select(log => log.CertificateData);

            var latestSubmittedFailCertificate = submittedCertificates.Where(submittedCert => submittedCert.OverallGrade == CertificateGrade.Fail)
                .OrderByDescending(submittedCert => submittedCert.AchievementDate)
                .FirstOrDefault();

            return latestSubmittedFailCertificate;
        }
    }
}
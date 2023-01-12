using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateCheckViewModel : CertificateSendToViewModel
    {
        public string Option { get; set; }
        public string Version { get; set; }
        public string SelectedGrade { get; set; }
        public string SelectedUkPrn { get; set; }
        public string SelectedStandard { get; set; }
        public DateTime? AchievementDate { get; set; }
        public DateTime? LearnerStartDate { get; set; }
        public bool StandardHasSingleOption { get; set; }
        public bool StandardHasOptions { get; set; }
        public bool StandardHasSingleVersion { get; set; }

        public string Name { get; set; }
        public string Dept { get; set; }
        public string Employer { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public string FirstName { get; set; }

        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);

            Level = CertificateData.StandardLevel;
            Option = CertificateData.CourseOption;
            Version = CertificateData.Version;
            SelectedGrade = CertificateData.OverallGrade;
            SelectedStandard = cert.StandardCode.ToString();
            SelectedUkPrn = cert.ProviderUkPrn.ToString();
            FirstName = CertificateData.LearnerGivenNames;
            LearnerStartDate = CertificateData.LearningStartDate;

            AchievementDate = CertificateData.AchievementDate;
            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;
            Employer = CertificateData.ContactOrganisation;
            AddressLine1 = CertificateData.ContactAddLine1;
            AddressLine2 = CertificateData.ContactAddLine2;
            AddressLine3 = CertificateData.ContactAddLine3;
            City = CertificateData.ContactAddLine4;
            Postcode = CertificateData.ContactPostCode;
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certificate = base.GetCertificateFromViewModel(certificate, certData);

            certificate.Status = CertificateStatus.Submitted;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }

        public void SetStandardHasVersionsAndOptions(CertificateSession certSession)
        {
            StandardHasOptions = certSession.Options != null && certSession.Options.Any();
            StandardHasSingleOption = certSession.Options == null || certSession.Options.Count <= 1;
            StandardHasSingleVersion = certSession.Versions == null || certSession.Versions.Count <= 1;
        }
    }
}
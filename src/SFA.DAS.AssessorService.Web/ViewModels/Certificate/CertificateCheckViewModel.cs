using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateCheckViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Option { get; set; }
        public string SelectedGrade { get; set; }
        public string SelectedUkPrn { get; set; }
        public string SelectedStandard { get; set; }
        public DateTime? AchievementDate { get; set; }
        public DateTime? LearnerStartDate { get; set; }

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

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.Status = certificate.IsPrivatelyFunded ? CertificateStatus.ToBeApproved : CertificateStatus.Submitted;
            certificate.PrivatelyFundedStatus = null;
            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}
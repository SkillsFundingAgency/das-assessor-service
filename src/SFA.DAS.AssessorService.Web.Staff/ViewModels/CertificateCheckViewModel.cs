using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateCheckViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public long Uln { get; set; }
        public string CertificateReference { get; set; }
        public int? Ukprn { get; set; }
        public int Level { get; set; }
        public string Option { get; set; }
        public string SelectedGrade { get; set; }
        public int StandardCode { get; set; }
        public DateTime? AchievementDate { get; set; }
        public DateTime? LearnerStartDate { get; set; }
        
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public string Employer { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Status { get; set; }
        public bool RedirectToCheck { get; set; }

        public string SearchString { get; set; }
        public int Page { get; set; }

        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);

            Uln = cert.Uln;
            Ukprn = cert.ProviderUkPrn;

            Status = cert.Status;
            PrivatelyFundedStatus = cert.PrivatelyFundedStatus;
            CertificateReference = cert.CertificateReference;

            Level = CertificateData.StandardLevel;
            Option = CertificateData.CourseOption;
            SelectedGrade = CertificateData.OverallGrade;
            StandardCode = cert.StandardCode;
            AchievementDate = CertificateData.AchievementDate;
            LearnerStartDate = CertificateData.LearningStartDate;

            FirstName = CertificateData.LearnerGivenNames;

            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;
            Employer = CertificateData.ContactOrganisation;
            AddressLine1 = CertificateData.ContactAddLine1;
            AddressLine2 = CertificateData.ContactAddLine2;
            AddressLine3 = CertificateData.ContactAddLine3;
            City = CertificateData.ContactAddLine4;
            Postcode = CertificateData.ContactPostCode;         
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            certificate.Status = CertificateStatus.Submitted;
            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}
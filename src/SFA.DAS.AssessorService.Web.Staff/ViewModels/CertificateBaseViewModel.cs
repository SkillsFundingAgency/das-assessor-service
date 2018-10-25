using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateBaseViewModel
    {
        protected CertificateData CertificateData;
        
        protected void BaseFromCertificate(Certificate cert)
        {
            CertificateData = JsonConvert.DeserializeObject<CertificateData>(cert.CertificateData);
            Id = cert.Id;
            GivenNames = CertificateData.LearnerGivenNames;
            FamilyName = CertificateData.LearnerFamilyName;
            Standard = CertificateData.StandardName;
            FullName = CertificateData.FullName;
            Level = CertificateData.StandardLevel;
            IsPrivatelyFunded = cert.IsPrivatelyFunded;
        }
        public Guid Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string Standard { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int Level { get; set; }    
        public bool IsPrivatelyFunded { get; set; }
        public bool BackToCheckPage { get; set; }
    }
}
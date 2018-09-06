using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateBaseViewModel
    {
        protected CertificateData CertificateData;
        
        protected void BaseFromCertificate(Domain.Entities.Certificate cert)
        {
            CertificateData = JsonConvert.DeserializeObject<CertificateData>(cert.CertificateData);
            Id = cert.Id;
            GivenNames = CertificateData.LearnerGivenNames;
            FamilyName = CertificateData.LearnerFamilyName;
            FullName = CertificateData.FullName;
            Standard = CertificateData.StandardName;
            IsPrivatelyFunded = cert.IsPrivatelyFunded;
        }
        public Guid Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string FullName { get; set; }
        public string Standard { get; set; }
        public bool BackToCheckPage { get; set; }
        public bool IsPrivatelyFunded { get; set; }
    }
}
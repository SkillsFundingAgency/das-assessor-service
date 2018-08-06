using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
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
            Standard = CertificateData.StandardName;
        }
        public Guid Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string Standard { get; set; }
        public string Username { get; set; }
        public bool BackToCheckPage { get; set; }
    }
}
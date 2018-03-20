using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateBaseViewModel
    {
        protected CertificateData CertificateData;

        public CertificateBaseViewModel() { }

        public CertificateBaseViewModel(Domain.Entities.Certificate cert)
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
    }
}
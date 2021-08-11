using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public abstract class CertificateBaseViewModel
    {
        protected CertificateData CertificateData;

        public abstract Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData);
        public virtual void FromCertificate(Domain.Entities.Certificate cert)
        {
            CertificateData = JsonConvert.DeserializeObject<CertificateData>(cert.CertificateData);
            Id = cert.Id;
            GivenNames = CertificateData.LearnerGivenNames;
            FamilyName = CertificateData.LearnerFamilyName;
            FullName = CertificateData.FullName;
            StandardReference = CertificateData.StandardReference;
            Standard = CertificateData.StandardName;
            Level = CertificateData.StandardLevel;
            Uln = cert.Uln.ToString();
            StandardUId = cert.StandardUId;
        }

        public Guid Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string FullName { get; set; }
        public string StandardReference { get; set; }
        public string Standard { get; set; }
        public string StandardUId { get; set; }
        public string Uln { get; set; }
        public int Level { get; set; }
        public bool BackToCheckPage { get; set; }
    }
}
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Runtime.ConstrainedExecution;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public abstract class CertificateBaseViewModel
    {
        protected CertificateData CertificateData;

        public abstract Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData);
        
        public virtual void FromCertificate(Domain.Entities.Certificate certificate)
        {
            CertificateData = certificate.CertificateData;

            Id = certificate.Id;
            GivenNames = CertificateData.LearnerGivenNames;
            FamilyName = CertificateData.LearnerFamilyName;
            FullName = CertificateData.FullName;
            StandardReference = CertificateData.StandardReference;
            Standard = CertificateData.StandardName;
            Level = CertificateData.StandardLevel;
            Uln = certificate.Uln.ToString();
            StandardUId = certificate.StandardUId;
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
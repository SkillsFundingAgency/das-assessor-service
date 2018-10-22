using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Ilr
    {
        public Guid Id { get; set; }
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [NotMapped] public string FamilyNameForSearch { get; set; }

        public int UkPrn { get; set; }
        public int StdCode { get; set; }
        public DateTime LearnStartDate { get; set; }
        public string EpaOrgId { get; set; }

        public int FundingModel { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long? EmployerAccountId { get; set; }
        public string Source { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string LearnRefNumber { get; set; }

        public Ilr GetFromCertificate(Certificate certificate)
        {            
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            return new Ilr
            {
                Id = certificate.Id,
                Uln = certificate.Uln,
                StdCode = certificate.StandardCode,
                UkPrn = certificate.ProviderUkPrn,
                FamilyName = certificateData.LearnerFamilyName,
                GivenNames = certificateData.LearnerGivenNames,
                CreatedAt = certificate.CreatedAt,
                EpaOrgId = certificate.Organisation.EndPointAssessorOrganisationId,
                LearnRefNumber = certificate.LearnRefNumber,
                LearnStartDate = certificateData.LearningStartDate ?? DateTime.MinValue,
                UpdatedAt = certificate.UpdatedAt,
                FamilyNameForSearch = certificateData.FullName
            };
        }
    }
}
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class CertificateBase : BaseEntity
    {
        public Guid Id { get; set; }
        public string Type { get; set; }

        public string CertificateReference { get; set; }
        public int? ProviderUkPrn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? CertificateReferenceId { get; set; }

        public int? BatchNumber { get; set; }

        public CertificateData CertificateData { get; set; }

        public string Status { get; set; }

        public DateTime? ToBePrinted { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }

        public DateTime CreateDay { get; set; }
        
        public virtual ICollection<CertificateLog> CertificateLogs { get; set; } = new List<CertificateLog>();

        [JsonIgnore]
        public virtual CertificateBatchLog CertificateBatchLog { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string LearnerFamilyName { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string LearnerGivenNames { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string LearnerFullNameNoSpaces { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullName { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactOrganisation { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ProviderName { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactName { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CourseOption { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string OverallGrade { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string StandardReference { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string StandardName { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Version { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int StandardLevel { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? AchievementDate { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LearningStartDate { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactAddLine1 { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactAddLine2 { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactAddLine3 { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactAddLine4 { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactPostCode { get; private set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string LatestEPAOutcome { get; private set; }
    }
}
using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class FrameworkLearner
    {
        public Guid Id { get; set; }
        public long CertificateReference { get; set; }
        public int CertificationYear { get; set; }
        public DateTime CertificationDate { get; set; }
        public string FrameworkName { get; set; }
        public string ApprenticeTitle { get; set; }  
        public string ApprenticeFullname { get; set; }
        public string ApprenticeSurname { get; set; }
        public string ApprenticeForename { get; set; }
        public string ApprenticeMiddlename { get; set; } 
        public DateTime ApprenticeDoB { get; set; }
        public long? ApprenticeUniqueNumber { get; set; } 
        public string CentreName { get; set; }
        public string IssuingAuthority { get; set; }
        public string Ukprn { get; set; } 
        public string EmployerName { get; set; } 
        public int? CompetenceQualificationId { get; set; } 
        public string CompetenceQualification { get; set; } 
        public string CompetenceQualificationNumber { get; set; } 
        public string CompetanceAwardingBody { get; set; } 
        public int? KnowledgeQualificationId { get; set; }
        public string KnowledgeQualification { get; set; }
        public string KnowledgeQualificationNumber { get; set; }
        public string KnowledgeAwardingBody { get; set; }
        public int? CombinedQualificationId { get; set; }
        public string CombinedQualification { get; set; }
        public string CombinedQualificationNumber { get; set; }
        public string CombinedAwardingBody { get; set; }
        public DateTime? ApprenticeStartdate { get; set; } 
        public DateTime? ApprenticeEnddate { get; set; }
        public DateTime? ApprenticeLastdateInLearning { get; set; }
        public string ApprenticeshipSector { get; set; }
        public int? EnglishId { get; set; }
        public string EnglishSkill { get; set; }
        public int? MathsId { get; set; }
        public string MathsSkill { get; set; }
        public int? IctId { get; set; }
        public string IctSkill { get; set; }
        public string Framework { get; set; }
        public string ApprenticeshipPathway { get; set; }
        public string ApprenticeshipLevel { get; set; }
        public long ApprenticeId { get; set; }
        public DateTime CreatedOn { get; set; } 
        public string ApprenticeNameMatch { get; set; } 
    }
}
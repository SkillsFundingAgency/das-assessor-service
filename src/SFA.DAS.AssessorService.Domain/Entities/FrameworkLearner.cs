using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class FrameworkLearner
    {
        public long ApprenticeId { get; set; }
        public DateTime ApprenticeDoB { get; set; }
        public DateTime? ApprenticeEnddate { get; set; }
        public string ApprenticeForename { get; set; }
        public string ApprenticeFullname { get; set; }
        public DateTime? ApprenticeLastdateInLearning { get; set; }
        public string ApprenticeMiddlename { get; set; }
        public string ApprenticeNameMatch { get; set; }
        public string FrameworkCertificateNumber { get; set; }
        public DateTime? ApprenticeStartdate { get; set; }
        public string ApprenticeSurname { get; set; }
        public string ApprenticeTitle { get; set; }
        public long? ApprenticeULN { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public string ApprenticeshipLevelName { get; set; }
        public string ApprenticeshipSector { get; set; }
        public string CentreName { get; set; }
        public DateTime CertificationDate { get; set; }
        public string CertificationYear { get; set; }
        public string CombinedAwardingBody { get; set; }
        public int? CombinedQualificationId { get; set; }
        public string CombinedQualification { get; set; }
        public string CombinedQualificationNumber { get; set; }
        public string CompetanceAwardingBody { get; set; }
        public int? CompetenceQualificationId { get; set; }
        public string CompetenceQualification { get; set; }
        public string CompetenceQualificationNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string EmployerName { get; set; }
        public int? EnglishId { get; set; }
        public string EnglishSkill { get; set; }
        public string Framework { get; set; }
        public string FrameworkName { get; set; }
        public int? IctId { get; set; }
        public string IctSkill { get; set; }
        public Guid Id { get; set; }
        public string IssuingAuthority { get; set; }
        public string KnowledgeAwardingBody { get; set; }
        public int? KnowledgeQualificationId { get; set; }
        public string KnowledgeQualification { get; set; }
        public string KnowledgeQualificationNumber { get; set; }
        public int? MathsId { get; set; }
        public string MathsSkill { get; set; }
        public string Pathway { get; set; }
        public string PathwayName { get; set; }
        public string ProviderName { get; set; }
        public string TrainingCode { get; set; }
        public string Ukprn { get; set; }
        public bool UlnConfirmed { get; set; }
    }
}
using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class FrameworkLearnerModel : TestModel
    {
        public Guid Id { get; set; }
        public string FrameworkCertificateNumber { get; set; }
        public string CertificationYear { get; set; }
        public DateTime CertificationDate { get; set; }
        public string ApprenticeFullname { get; set; }
        public string ApprenticeSurname { get; set; }
        public string ApprenticeForename { get; set; }
        public DateTime ApprenticeDoB { get; set; }
        public long ApprenticeULN { get; set; }
        public string TrainingCode { get; set; }
        public string FrameworkName { get; set; }
        public string PathwayName { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public string ProviderName { get; set; }
        public string Ukprn { get; set; }
        public string Framework { get; set; }
        public string Pathway { get; set; }
        public string ApprenticeshipLevelName { get; set; }
        public long ApprenticeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ApprenticeNameMatch { get; set; }
    }
}

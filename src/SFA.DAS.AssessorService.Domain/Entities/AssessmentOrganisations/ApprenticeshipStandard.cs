using System;

namespace SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations
{
    public class ApprenticeshipStandard
    {
        public int Id { get; set; }
        public string StandardCode { get; set; }
        public int Version { get; set; }
        public string StandardName { get; set; }
        public int StandardSectorCode { get; set; }
        public int NotionalEndLevel { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? LastDateStarts { get; set; }
        public string UrlLink { get; set; }
        public int? SectorSubjectAreaTier1 { get; set; }
        public string SectorSubjectAreaTier2 { get; set; }

        public bool? IntegratedDegreeStandard { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Status { get; set; }
    }
}

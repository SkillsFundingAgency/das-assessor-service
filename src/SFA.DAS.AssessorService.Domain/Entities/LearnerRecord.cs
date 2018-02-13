using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class LearnerRecord
    {
        [Key]
        public long LearnerRecordId { get; set; }
        public string ULN { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string UkPrn { get; set; }
        public string StandardCode { get; set; }
        public DateTime LearningStartDate { get; set; }
        public string EPAOrgId { get; set; }
        public long? OrganisationId { get; set; }
        [ForeignKey("OrganisationId")]
        public Organisation Organisation { get; set; }
        public string Outcome { get; set; }
        public DateTime? AchievementDate { get; set; }
        public string OutcomeGrade { get; set; }
    }
}
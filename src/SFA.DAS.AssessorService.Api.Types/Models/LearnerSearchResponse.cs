using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class LearnerSearchResponse
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int StdCode { get; set; }
        public string StandardReferenceNumber { get; set; }
        public List<StandardVersion> Versions { get; set; }
        public string Standard { get; set; }
        public int Level { get; set; }
        public bool CoronationEmblem { get; set; }
        public int? FundingModel { get; set; }
        public int UkPrn { get; set; }
        public string Version { get; set; }
        public bool VersionConfirmed { get; set; }
        public string StandardUId { get; set; }
        public string Option { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public string OverallGrade { get; set; }
        public DateTime? AchDate { get; set; }
        public Guid CertificateId { get; set; }
        public string CertificateReference { get; set; }
        public string CertificateStatus { get; set; }
        public bool IsPrivatelyFunded { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool ShowExtraInfo { get; set; }
        public bool UlnAlreadyExits { get; set; }
        public bool IsNoMatchingFamilyName { get; set; }
    }
}
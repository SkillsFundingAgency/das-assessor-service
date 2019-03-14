﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Ilr
    {
        public Guid Id { get; set; }
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [NotMapped]
        public string FamilyNameForSearch { get; set; }

        public int UkPrn { get; set; }
        public int StdCode { get; set; }
        public DateTime LearnStartDate { get; set; }
        public string EpaOrgId { get; set; }

        public int? FundingModel { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long? EmployerAccountId { get; set; }
        public string Source { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string LearnRefNumber { get; set; }
        public int? CompletionStatus { get; set; }
        public long? EventId { get; set; }
        public DateTime? PlannedEndDate { get; set; }
    }
}
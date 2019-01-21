﻿using System;

namespace SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types
{
    public class IfaStandard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }

        // From GetAllStandards from IFA
        public int? Duration { get; set; }
        public int? MaxFunding { get; set; }
        public DateTime? PublishedDate { get; set; }

        public string Ssa1 { get; set; }
        public string Ssa2 { get; set; }
       

        // From /standards/{id} on IFA
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string OverviewOfRole { get; set; }
        public bool IsPublished { get; set; }
        public string Uri { get; set; }

        public string AssessmentPlanUrl { get; set; }
    }
}
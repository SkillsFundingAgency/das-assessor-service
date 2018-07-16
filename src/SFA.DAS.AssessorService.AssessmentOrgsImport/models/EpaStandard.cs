﻿using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.models
{
    public class EpaOrganisationStandard
    {
        public int Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string StandardCode { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string ContactName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }        
        public string Status { get; set; }
    }
}

﻿using System;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class CertificateForSearch
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public long Uln { get; set; }
        public string CertificateReference { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string Status { get; set; }     
        public DateTime? LastUpdatedAt { get; set; }
    }
}
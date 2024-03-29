﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class OrganisationStandardVersion
    {
        public string StandardUId { get; set; }

        [NotMapped]
        public string Title { get; set; }
        [NotMapped]
        public int LarsCode { get; set; }
        [NotMapped]
        public string IFateReferenceNumber { get; set; }


        public string Version { get; set; }

        [NotMapped]
        public int Level { get; set; }

        public int OrganisationStandardId { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public OrganisationStandard OrganisationStandard { get; set; }
    }
}
﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Certificate : BaseEntity
    {
        public Guid Id { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public int ProviderUkPrn { get; set; }
        public Guid OrganisationId { get; set; }
        public Organisation Organisation { get; set; }

        public string CertificateReference { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? CertificateReferenceId { get; set; }

        public int? BatchNumber { get; set; }

        public string CertificateData { get; set; }

        public string Status { get; set; }

        public DateTime ToBePrinted { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
    }
}
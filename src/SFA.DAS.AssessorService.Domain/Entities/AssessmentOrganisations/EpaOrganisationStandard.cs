using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations
{
    public class EpaOrganisationStandard
    {
        public int Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string ContactName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }        
        public string Status { get; set; }

        public Guid? ContactId { get; set; }
        public OrganisationStandardData OrganisationStandardData { get; set; }

        [NotMapped]
        public string OrganisationStandardDataJsonString { get => JsonConvert.SerializeObject(OrganisationStandardData); }
    }
}

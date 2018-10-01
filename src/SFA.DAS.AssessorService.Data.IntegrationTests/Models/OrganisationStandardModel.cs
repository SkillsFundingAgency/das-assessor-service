using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class OrganisationStandardModel: TestModel
    {
        public int Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}

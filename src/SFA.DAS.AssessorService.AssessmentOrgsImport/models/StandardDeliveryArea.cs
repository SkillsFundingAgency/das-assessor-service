using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.models
{
    public class EpaOrganisationStandardDeliveryArea
    {
        public Guid Id { get; set; }
        public string EpaOrganisationIdentifier { get; set; }
        public int StandardCode { get; set; }
        public Guid DeliveryAreaId { get; set; }
        public string Comments { get; set; }
    }
}

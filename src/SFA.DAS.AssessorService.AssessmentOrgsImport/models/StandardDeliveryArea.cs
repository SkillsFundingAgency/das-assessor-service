using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.models
{
    public class EpaOrganisationStandardDeliveryArea
    {
        public int Id { get; set; }
        public string EpaOrganisationIdentifier { get; set; }
        public string StandardCode { get; set; }
        public int DeliveryAreaId { get; set; }
        public string Comments { get; set; }
    }
}

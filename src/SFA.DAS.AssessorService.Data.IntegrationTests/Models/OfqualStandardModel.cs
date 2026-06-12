using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class OfqualStandardModel : TestModel
    { 
        public Guid? Id { get; set; }

        public string RecognitionNumber { get; set; }

        public DateTime OperationalStartDate { get; set; }

        public DateTime? OperationalEndDate { get; set; } 

        public string IfateReferenceNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

}

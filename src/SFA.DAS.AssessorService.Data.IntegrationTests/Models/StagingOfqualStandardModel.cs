using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class StagingOfqualStandardModel : TestModel
    {
        public string RecognitionNumber { get; set; }

        public DateTime OperationalStartDate { get; set; }

        public DateTime? OperationalEndDate { get; set; } // Nullable DateTime

        public string IfateReferenceNumber { get; set; }
    }

}

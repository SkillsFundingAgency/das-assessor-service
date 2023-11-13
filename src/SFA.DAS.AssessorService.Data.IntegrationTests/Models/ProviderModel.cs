using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class ProviderModel : TestModel
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}

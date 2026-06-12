using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class OfsOrganisationModel : TestModel
    {
        public Guid? Id { get; set; }
        public int Ukprn { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

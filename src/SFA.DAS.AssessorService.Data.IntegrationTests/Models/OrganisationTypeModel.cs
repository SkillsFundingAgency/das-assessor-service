using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class OrganisationTypeModel: TestModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}

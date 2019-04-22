using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class OrganisationType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string TypeDescription { get; set; }
    }
}
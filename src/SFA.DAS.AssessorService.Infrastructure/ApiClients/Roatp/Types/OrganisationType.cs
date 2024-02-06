using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types
{
    public class OrganisationType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public const int Unassigned = 0;
    }
}

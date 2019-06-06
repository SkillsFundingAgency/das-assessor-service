using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.Entities
{
    public class Organisation
    {
        public Guid Id { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }

        public string PrimaryContact { get; set; }

        public bool ApiEnabled { get; set; }
        public string ApiUser { get; set; }

        public string Status { get; set; }

        public string OrganisationData { get; set; }

        public int? OrganisationTypeId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.Entities
{
    public class OrganisationStandard
    {
        public int Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public Guid? ContactId { get; set; }
        public string OrganisationStandardData { get; set; }
    }
}

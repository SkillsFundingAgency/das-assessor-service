using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class RegisterViewAndEditOrganisationStandardViewModel
    {
        public int OrganisationStandardId { get; set; }
        public string OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public string OrganisationStatus { get; set; }
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public Guid? ContactId { get; set; }
        public EpaContact Contact { get; set; }
        public List<OrganisationStandardDeliveryArea> DeliveryAreas { get; set; }
    }
}

using SFA.DAS.AssessorService.Api.Types.Models;
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

        public long? Ukprn { get; set; }
        public DateTime? StandardEffectiveFrom { get; set; }
        public DateTime? StandardEffectiveTo { get; set; }
        public DateTime? StandardLastDateForNewStarts { get; set; }

        public string OrganisationName { get; set; }
        public string OrganisationStatus { get; set; }
        public int StandardId { get; set; }
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public Guid? ContactId { get; set; }
        public EpaContact Contact { get; set; }
        public List<int> DeliveryAreas { get; set; }

        public List<OrganisationStandardDeliveryArea> DeliveryAreasDetails { get; set; }
        public string EffectiveFromDay { get; set; }
        public string EffectiveFromMonth { get; set; }
        public string EffectiveFromYear { get; set; }
        public string EffectiveToDay { get; set; }
        public string EffectiveToMonth { get; set; }
        public string EffectiveToYear { get; set; }


        public List<ContactResponse> Contacts { get; set; }
        public List<DeliveryArea> AvailableDeliveryAreas { get; set; }

        public string ActionChoice { get; set; }

        public string DeliveryAreasComments { get; set; }
    }
}

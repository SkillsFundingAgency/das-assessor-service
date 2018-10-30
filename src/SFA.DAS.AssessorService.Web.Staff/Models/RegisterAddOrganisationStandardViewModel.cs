using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class RegisterAddOrganisationStandardViewModel
    {
        public string OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public long? Ukprn { get; set; }

        public int StandardId { get; set; }
        public string StandardTitle { get; set; }
        public DateTime? StandardEffectiveFrom { get; set; }
        public DateTime? StandardEffectiveTo { get; set; }
        public DateTime? StandardLastDateForNewStarts { get; set; }
        public List<ContactResponse> Contacts { get; set; }
        public List<DeliveryArea> AvailableDeliveryAreas { get; set; }
        //public DateTime EffectiveFromDate { get; set; }
        public string EffectiveFromDay { get; set; }
        public string EffectiveFromMonth { get; set; }
        public string EffectiveFromYear { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public string EffectiveToDay { get; set; }
        public string EffectiveToMonth { get; set; }
        public string EffectiveToYear { get; set; }
        public DateTime? EffectiveTo {get;set;}
        public Guid? ContactId { get; set; }
        public List<int> DeliveryAreas { get; set; }
        public string Comments { get; set; }

    }
}

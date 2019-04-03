using System.Runtime.Serialization;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{

    using System;

    public class UpdateOrganisationUkprnViewModel
    {
        [DataMember]
        public string Ukprn { get; set; }
        [DataMember]
        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}

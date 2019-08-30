using SFA.DAS.AssessorService.Api.Types.Attributes;
using SFA.DAS.AssessorService.Api.Types.Auditing;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    [AuditFilter(typeof(EpaOrganisationAuditFilter))]
    public class EpaOrganisation //: IAuditFilter
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Name { get; set; }
        public string OrganisationId { get; set; }
        public long? Ukprn { get; set; }
        public int? OrganisationTypeId { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactName { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedAt { get; set; }     
        public OrganisationData OrganisationData { get; set; }
        public bool ApiEnabled { get; set; }
        public string ApiUser { get; set; }

        /*public string FilterAuditDiff(string propertyChanged)
        {
            if (propertyChanged == $"{nameof(OrganisationData)}.{nameof(OrganisationData.PhoneNumber)}")
            {
                return "Contact number";
            }

            return null;
        }*/
    }
}

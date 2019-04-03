using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Commands
{
    public class CreateOrganisationStandardCommand
    {
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime DateStandardApprovedOnRegister { get; } = DateTime.Now.Date;
        public List<string> DeliveryAreas { get; set; }

        public string CreatedBy { get; set; }

        public CreateOrganisationStandardCommand(string createdBy,
            string organisationId, int standardCode, DateTime effectiveFrom, List<string> deliveryAreas)
        {
            CreatedBy = createdBy;
            OrganisationId = organisationId;
            StandardCode = standardCode;
            EffectiveFrom = effectiveFrom;
            DeliveryAreas = deliveryAreas;
        }

        public CreateOrganisationStandardCommand()
        {
        }
    }
}

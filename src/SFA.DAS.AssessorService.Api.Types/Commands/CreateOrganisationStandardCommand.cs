using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Commands
{
    public class CreateOrganisationStandardCommand
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime DateStandardApprovedOnRegister { get; } = DateTime.Now.Date;
        public List<string> DeliveryAreas { get; set; }

        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public bool UseTradingName { get; set; }
        public string CreatedBy { get; set; }

        public CreateOrganisationStandardCommand(string organisationName, string tradingName, bool useTradingName, string createdBy,
            string endPointAssessorOrganisationId, int standardCode, DateTime effectiveFrom, List<string> deliveryAreas)
        {
            OrganisationName = organisationName;
            TradingName = tradingName;
            UseTradingName = useTradingName;
            CreatedBy = createdBy;

            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            StandardCode = standardCode;
            EffectiveFrom = effectiveFrom;
            DeliveryAreas = deliveryAreas;
        }

        public CreateOrganisationStandardCommand()
        {
        }
    }
}

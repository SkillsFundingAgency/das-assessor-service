namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using MediatR;

    public class UpdateOrganisationTradingNameRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string TradingName { get; set; }
        public string UpdatedBy { get; set; }
    }
}

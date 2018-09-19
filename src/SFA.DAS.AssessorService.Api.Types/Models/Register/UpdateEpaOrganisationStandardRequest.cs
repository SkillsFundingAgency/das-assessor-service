using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationStandardRequest : IRequest<string>
    {
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Comments { get; set; }
        public string ContactId { get; set; }
    }
}

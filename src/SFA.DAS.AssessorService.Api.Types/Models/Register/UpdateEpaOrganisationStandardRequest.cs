using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationStandardRequest : IRequest<int>
    {
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}

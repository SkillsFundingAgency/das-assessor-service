using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class UpdateOrganisationCharityNumberRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public string CharityNumber { get; set; }
        public string UpdatedBy { get; set; }
    }
}

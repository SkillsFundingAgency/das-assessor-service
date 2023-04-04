using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class DuplicateCharityNumberCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string CharityNumber { get; set; }
    }
}

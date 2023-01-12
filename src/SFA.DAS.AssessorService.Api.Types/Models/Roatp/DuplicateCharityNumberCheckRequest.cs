namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using MediatR;
    using System;

    public class DuplicateCharityNumberCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string CharityNumber { get; set; }
    }
}

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using MediatR;
    using System;

    public class DuplicateUKPRNCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string UKPRN { get; set; }
    }
}

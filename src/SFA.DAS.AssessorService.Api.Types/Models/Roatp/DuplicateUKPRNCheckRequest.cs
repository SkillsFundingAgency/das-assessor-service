namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using MediatR;

    public class DuplicateUKPRNCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string UKPRN { get; set; }
    }
}

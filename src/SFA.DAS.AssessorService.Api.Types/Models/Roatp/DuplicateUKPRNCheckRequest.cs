using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class DuplicateUKPRNCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string UKPRN { get; set; }
    }
}

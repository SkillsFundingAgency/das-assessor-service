using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class UpdateOrganisationUkprnRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string Ukprn { get; set; }
        public string UpdatedBy { get; set; }
    }
}

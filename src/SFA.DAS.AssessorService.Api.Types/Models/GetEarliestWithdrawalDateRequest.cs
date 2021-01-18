using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEarliestWithdrawalDateRequest : IRequest<DateTime>
    {
        public GetEarliestWithdrawalDateRequest(Guid organisationId, int? standardId)
        {
            OrganisationId = organisationId;
            StandardId = standardId;
        }

        public Guid OrganisationId { get; }
        public int? StandardId { get; set; }
    }
}

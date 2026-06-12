using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetLatestWithdrawalDateForStandardRequest : IRequest<DateTime?>
    {
        public Guid OrganisationId { get; }
        public int? StandardCode { get; }

        public GetLatestWithdrawalDateForStandardRequest(Guid organisationId, int? standardCode)
        {
            OrganisationId = organisationId;
            StandardCode = standardCode;
        }
    }
}

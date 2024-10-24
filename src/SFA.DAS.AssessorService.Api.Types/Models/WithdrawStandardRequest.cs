using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class WithdrawStandardRequest : IRequest<Unit>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime WithdrawalDate { get; set; }
    }
}

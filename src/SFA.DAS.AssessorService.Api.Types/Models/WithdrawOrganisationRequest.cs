using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class WithdrawOrganisationRequest : IRequest
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public DateTime WithdrawalDate { get; set; }
    }
}

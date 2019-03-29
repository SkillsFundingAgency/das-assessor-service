using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class UpdateOrganisationFinancialTrackRecordRequest: IRequest
    {
        public Guid OrganisationId { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public string UpdatedBy { get; set; }
    }
}

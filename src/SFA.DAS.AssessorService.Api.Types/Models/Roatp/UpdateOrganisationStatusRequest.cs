namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using MediatR;

    public class UpdateOrganisationStatusRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public int OrganisationStatusId { get; set; }
        public int? RemovedReasonId { get; set; }
        public string UpdatedBy { get; set; }
    }
}

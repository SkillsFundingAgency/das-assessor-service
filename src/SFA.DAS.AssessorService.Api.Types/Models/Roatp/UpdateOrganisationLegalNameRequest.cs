namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using MediatR;

    public class UpdateOrganisationLegalNameRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        public string UpdatedBy { get; set; }
    }
}

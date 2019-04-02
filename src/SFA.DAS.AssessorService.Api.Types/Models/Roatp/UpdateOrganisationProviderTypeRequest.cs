namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using MediatR;
    using System;

    public class UpdateOrganisationProviderTypeRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public int ProviderTypeId { get; set; }
        public int OrganisationTypeId { get; set; }
        public string UpdatedBy { get; set; }
    }
}

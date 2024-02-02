using MediatR;
using System;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class MergeOrganisationsRequest : IRequest<Domain.Entities.MergeOrganisation>
    {
        public string PrimaryEndPointAssessorOrganisationId { get; set; }
        public string SecondaryEndPointAssessorOrganisationId { get; set; }
        public DateTime SecondaryStandardsEffectiveTo { get; set; }
        public string ActionedByUser { get; set; }
    }
}

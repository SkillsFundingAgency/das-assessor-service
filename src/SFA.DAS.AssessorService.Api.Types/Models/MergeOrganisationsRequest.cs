using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class MergeOrganisationsRequest : IRequest<MergeOrganisation>
    {
        public string PrimaryEndPointAssessorOrganisationId { get; set; }
        public string SecondaryEndPointAssessorOrganisationId { get; set; }
        public DateTime SecondaryStandardsEffectiveTo { get; set; }
        public string ActionedByUser { get; set; }
    }
}

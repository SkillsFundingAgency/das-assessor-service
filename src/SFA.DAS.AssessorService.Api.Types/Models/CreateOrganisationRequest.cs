namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;
    using MediatR;
    using Types;

    public class CreateOrganisationRequest : IRequest<Organisation>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
    }
}

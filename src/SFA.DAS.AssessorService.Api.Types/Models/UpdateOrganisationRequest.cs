namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;
    using MediatR;

    public class UpdateOrganisationRequest : IRequest<Organisation>
    {
        public string EndPointAssessorOrganisationId { get; set; }       
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
    }
}

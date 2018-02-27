namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using SFA.DAS.AssessorService.Api.Types;
    using System;

    public class CreateOrganisationRequest : IRequest<Organisation>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
    }
}

namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.Api.Types;

    public class UpdateOrganisationRequest : IRequest<Organisation>
    {
        public Guid Id { get; set; }       
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}

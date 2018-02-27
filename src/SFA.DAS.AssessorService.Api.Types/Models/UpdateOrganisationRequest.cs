namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;
    using SFA.DAS.AssessorService.Domain.Enums;

    public class UpdateOrganisationRequest : IRequest<Organisation>
    {
        public Guid Id { get; set; }       
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}

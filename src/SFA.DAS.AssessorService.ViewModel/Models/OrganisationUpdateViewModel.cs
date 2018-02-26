namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;
    using SFA.DAS.AssessorService.Domain.Enums;

    public class OrganisationUpdateViewModel : IRequest<OrganisationQueryViewModel>
    {
        public Guid Id { get; set; }       
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}

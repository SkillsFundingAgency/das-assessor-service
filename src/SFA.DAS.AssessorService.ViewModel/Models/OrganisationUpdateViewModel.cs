using SFA.DAS.AssessorService.Domain.Enums;

namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class OrganisationUpdateViewModel : IRequest<OrganisationQueryViewModel>
    {
        public Guid Id { get; set; }       
        public string EndPointAssessorName { get; set; }
        public int? PrimaryContactId { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}

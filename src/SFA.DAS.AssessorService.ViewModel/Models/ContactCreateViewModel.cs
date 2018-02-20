namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class ContactCreateViewModel : IRequest<ContactQueryViewModel>
    {
        public Guid OrganisationId { get; set; }

        public int EndPointAssessorContactId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
    }
}

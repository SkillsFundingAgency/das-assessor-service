namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class CreateContactRequest : IRequest<Contactl>
    {
        public Guid OrganisationId { get; set; }

        public int EndPointAssessorContactId { get; set; }    

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
    }
}

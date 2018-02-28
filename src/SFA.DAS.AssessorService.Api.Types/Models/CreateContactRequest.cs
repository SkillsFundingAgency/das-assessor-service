namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;
    using MediatR;

    public class CreateContactRequest : IRequest<Contact>
    {
        public Guid OrganisationId { get; set; }

        public int EndPointAssessorContactId { get; set; }    

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
    }
}

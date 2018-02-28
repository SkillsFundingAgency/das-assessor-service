namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;
    using MediatR;

    public class UpdateContactRequest : IRequest
    {
        public Guid Id { get; set; }
      
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }        
    }
}

namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class ContactUpdateViewModel : IRequest
    {
        public Guid Id { get; set; }
      
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }        
    }
}

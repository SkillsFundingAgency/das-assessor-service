namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class ContactDeleteViewModel : IRequest
    {
        public Guid Id { get; set; }                  
    }
}

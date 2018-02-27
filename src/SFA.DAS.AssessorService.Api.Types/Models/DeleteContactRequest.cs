namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class DeleteContactRequest : IRequest
    {
        public Guid Id { get; set; }                  
    }
}

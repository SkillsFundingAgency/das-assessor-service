namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class OrganisationDeleteViewModel : IRequest
    {
        public Guid Id { get; set; }                  
    }
}

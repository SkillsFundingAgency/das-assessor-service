namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;
    using System;

    public class DeleteOrgananisationRequest : IRequest
    {
        public Guid Id { get; set; }                  
    }
}

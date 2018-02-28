namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;
    using MediatR;

    public class DeleteOrgananisationRequest : IRequest
    {
        public Guid Id { get; set; }                  
    }
}
